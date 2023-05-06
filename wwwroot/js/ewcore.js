/*!
 * Core JavaScript for ASP.NET
 */
var ew = (function () {
  'use strict';

  /**
   * Global dependencies.
   * @global {Object} document - DOM
   */

  var devnull = function () {},
    bundleIdCache = {},
    bundleResultCache = {},
    bundleCallbackQueue = {};

  /**
   * Subscribe to bundle load event.
   * @param {string[]} bundleIds - Bundle ids
   * @param {Function} callbackFn - The callback function
   */
  function subscribe(bundleIds, callbackFn) {
    // listify
    bundleIds = bundleIds.push ? bundleIds : [bundleIds];
    var depsNotFound = [],
      i = bundleIds.length,
      numWaiting = i,
      fn,
      bundleId,
      r,
      q;

    // define callback function
    fn = function (bundleId, pathsNotFound) {
      if (pathsNotFound.length) depsNotFound.push(bundleId);
      numWaiting--;
      if (!numWaiting) callbackFn(depsNotFound);
    };

    // register callback
    while (i--) {
      bundleId = bundleIds[i];

      // execute callback if in result cache
      r = bundleResultCache[bundleId];
      if (r) {
        fn(bundleId, r);
        continue;
      }

      // add to callback queue
      q = bundleCallbackQueue[bundleId] = bundleCallbackQueue[bundleId] || [];
      q.push(fn);
    }
  }

  /**
   * Publish bundle load event.
   * @param {string} bundleId - Bundle id
   * @param {string[]} pathsNotFound - List of files not found
   */
  function publish(bundleId, pathsNotFound) {
    // exit if id isn't defined
    if (!bundleId) return;
    var q = bundleCallbackQueue[bundleId];

    // cache result
    bundleResultCache[bundleId] = pathsNotFound;

    // exit if queue is empty
    if (!q) return;

    // empty callback queue
    while (q.length) {
      q[0](bundleId, pathsNotFound);
      q.splice(0, 1);
    }
  }

  /**
   * Execute callbacks.
   * @param {Object or Function} args - The callback args
   * @param {string[]} depsNotFound - List of dependencies not found
   */
  function executeCallbacks(args, depsNotFound) {
    // accept function as argument
    if (args.call) args = {
      success: args
    };

    // success and error callbacks
    if (depsNotFound.length) (args.error || devnull)(depsNotFound);else (args.success || devnull)(args);
  }

  /**
   * Load individual file.
   * @param {string} path - The file path
   * @param {Function} callbackFn - The callback function
   */
  function loadFile(path, callbackFn, args, numTries) {
    var doc = document,
      async = args.async,
      maxTries = (args.numRetries || 0) + 1,
      beforeCallbackFn = args.before || devnull,
      pathname = path.replace(/[\?|#].*$/, ''),
      pathStripped = path.replace(/^(css|img)!/, ''),
      // isLegacyIECss, //***
      e;
    numTries = numTries || 0;
    if (/(^css!|\.css$)/.test(pathname)) {
      // css
      e = doc.createElement('link');
      e.rel = 'stylesheet';
      e.href = pathStripped;

      // tag IE9+
      // isLegacyIECss = 'hideFocus' in e; //***

      // use preload in IE Edge (to detect load errors)
      // if (isLegacyIECss && e.relList) { //***
      //   isLegacyIECss = 0;
      //   e.rel = 'preload';
      //   e.as = 'style';
      // }
    } else if (/(^img!|\.(png|gif|jpg|svg|webp)$)/.test(pathname)) {
      // image
      e = doc.createElement('img');
      e.src = pathStripped;
    } else {
      // javascript
      e = doc.createElement('script');
      e.src = path;
      e.async = async === undefined ? true : async;
    }
    e.onload = e.onerror = e.onbeforeload = function (ev) {
      var result = ev.type[0];

      // treat empty stylesheets as failures to get around lack of onerror
      // support in IE9-11
      // if (isLegacyIECss) { //***
      //   try {
      //     if (!e.sheet.cssText.length) result = 'e';
      //   } catch (x) {
      //     // sheets objects created from load errors don't allow access to
      //     // `cssText` (unless error is Code:18 SecurityError)
      //     if (x.code != 18) result = 'e';
      //   }
      // }

      // handle retries in case of load failure
      if (result == 'e') {
        // increment counter
        numTries += 1;

        // exit function and try again
        if (numTries < maxTries) {
          return loadFile(path, callbackFn, args, numTries);
        }
      } else if (e.rel == 'preload' && e.as == 'style') {
        // activate preloaded stylesheets
        return e.rel = 'stylesheet'; // jshint ignore:line
      }

      // execute callback
      callbackFn(path, result, ev.defaultPrevented);
    };

    // add to document (unless callback returns `false`)
    if (beforeCallbackFn(path, e) !== false && e.tagName != "IMG") doc.head.appendChild(e); //***
  }

  /**
   * Load multiple files.
   * @param {string[]} paths - The file paths
   * @param {Function} callbackFn - The callback function
   */
  function loadFiles(paths, callbackFn, args) {
    // listify paths
    paths = paths.push ? paths : [paths];
    var numWaiting = paths.length,
      x = numWaiting,
      pathsNotFound = [],
      fn,
      i;

    // define callback function
    fn = function (path, result, defaultPrevented) {
      // handle error
      if (result == 'e') pathsNotFound.push(path);

      // handle beforeload event. If defaultPrevented then that means the load
      // will be blocked (ex. Ghostery/ABP on Safari)
      if (result == 'b') {
        if (defaultPrevented) pathsNotFound.push(path);else return;
      }
      numWaiting--;
      if (!numWaiting) callbackFn(pathsNotFound);
    };

    // load scripts
    for (i = 0; i < x; i++) loadFile(paths[i], fn, args);
  }

  /**
   * Initiate script load and register bundle.
   * @param {(string|string[])} paths - The file paths
   * @param {(string|Function|Object)} [arg1] - The (1) bundleId or (2) success
   *   callback or (3) object literal with success/error arguments, numRetries,
   *   etc.
   * @param {(Function|Object)} [arg2] - The (1) success callback or (2) object
   *   literal with success/error arguments, numRetries, etc.
   */
  function loadjs(paths, arg1, arg2) {
    var bundleId, args;

    // bundleId (if string)
    if (arg1 && arg1.trim) bundleId = arg1;

    // args (default is {})
    args = (bundleId ? arg2 : arg1) || {};

    // throw error if bundle is already defined
    if (bundleId) {
      if (bundleId in bundleIdCache) {
        throw "LoadJS";
      } else {
        bundleIdCache[bundleId] = true;
      }
    }
    function loadFn(resolve, reject) {
      loadFiles(paths, function (pathsNotFound) {
        // execute callbacks
        executeCallbacks(args, pathsNotFound);

        // resolve Promise
        if (resolve) {
          executeCallbacks({
            success: resolve,
            error: reject
          }, pathsNotFound);
        }

        // publish bundle load event
        publish(bundleId, pathsNotFound);
      }, args);
    }
    if (args.returnPromise) return new Promise(loadFn);else loadFn();
  }

  /**
   * Execute callbacks when dependencies have been satisfied.
   * @param {(string|string[])} deps - List of bundle ids
   * @param {Object} args - success/error arguments
   */
  loadjs.ready = function ready(deps, args) {
    // subscribe to bundle load event
    subscribe(deps, function (depsNotFound) {
      // execute callbacks
      executeCallbacks(args, depsNotFound);
    });
    return loadjs;
  };

  /**
   * Manually satisfy bundle dependencies.
   * @param {string} bundleId - The bundle id
   */
  loadjs.done = function done(bundleId) {
    publish(bundleId, []);
  };

  /**
   * Reset loadjs dependencies statuses
   */
  loadjs.reset = function reset() {
    bundleIdCache = {};
    bundleResultCache = {};
    bundleCallbackQueue = {};
  };

  /**
   * Determine if bundle has already been defined
   * @param String} bundleId - The bundle id
   */
  loadjs.isDefined = function isDefined(bundleId) {
    return bundleId in bundleIdCache;
  };
   //***

  function _typeof(obj) {
    "@babel/helpers - typeof";

    return _typeof = "function" == typeof Symbol && "symbol" == typeof Symbol.iterator ? function (obj) {
      return typeof obj;
    } : function (obj) {
      return obj && "function" == typeof Symbol && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj;
    }, _typeof(obj);
  }

  function _toPrimitive(input, hint) {
    if (_typeof(input) !== "object" || input === null) return input;
    var prim = input[Symbol.toPrimitive];
    if (prim !== undefined) {
      var res = prim.call(input, hint || "default");
      if (_typeof(res) !== "object") return res;
      throw new TypeError("@@toPrimitive must return a primitive value.");
    }
    return (hint === "string" ? String : Number)(input);
  }

  function _toPropertyKey(arg) {
    var key = _toPrimitive(arg, "string");
    return _typeof(key) === "symbol" ? key : String(key);
  }

  function _defineProperties(target, props) {
    for (var i = 0; i < props.length; i++) {
      var descriptor = props[i];
      descriptor.enumerable = descriptor.enumerable || false;
      descriptor.configurable = true;
      if ("value" in descriptor) descriptor.writable = true;
      Object.defineProperty(target, _toPropertyKey(descriptor.key), descriptor);
    }
  }
  function _createClass(Constructor, protoProps, staticProps) {
    if (protoProps) _defineProperties(Constructor.prototype, protoProps);
    if (staticProps) _defineProperties(Constructor, staticProps);
    Object.defineProperty(Constructor, "prototype", {
      writable: false
    });
    return Constructor;
  }

  function _defineProperty(obj, key, value) {
    key = _toPropertyKey(key);
    if (key in obj) {
      Object.defineProperty(obj, key, {
        value: value,
        enumerable: true,
        configurable: true,
        writable: true
      });
    } else {
      obj[key] = value;
    }
    return obj;
  }

  /**
   * Language class
   */
  let Language = /*#__PURE__*/function () {
    /**
     * Constructor
     * @param {Object} data Phrases
     */
    function Language(data) {
      _defineProperty(this, "data", {});
      this.data = data != null ? data : {};
    }

    /**
     * Get phrase by ID
     *
     * @param {string} id ID
     * @returns {string} Phrase
     */
    var _proto = Language.prototype;
    _proto.phrase = function phrase(id) {
      var _this$data$id$toLower;
      return (_this$data$id$toLower = this.data[id.toLowerCase()]) != null ? _this$data$id$toLower : id;
    }

    /**
     * Get phrases
     */;
    _createClass(Language, [{
      key: "phrases",
      get: function () {
        return this.data;
      }

      /**
       * Set phrases
       */,
      set: function (value) {
        this.data = value;
      }
    }]);
    return Language;
  }();

  function _assertThisInitialized(self) {
    if (self === void 0) {
      throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
    }
    return self;
  }

  function _setPrototypeOf(o, p) {
    _setPrototypeOf = Object.setPrototypeOf ? Object.setPrototypeOf.bind() : function _setPrototypeOf(o, p) {
      o.__proto__ = p;
      return o;
    };
    return _setPrototypeOf(o, p);
  }

  function _inheritsLoose(subClass, superClass) {
    subClass.prototype = Object.create(superClass.prototype);
    subClass.prototype.constructor = subClass;
    _setPrototypeOf(subClass, superClass);
  }

  function _getPrototypeOf(o) {
    _getPrototypeOf = Object.setPrototypeOf ? Object.getPrototypeOf.bind() : function _getPrototypeOf(o) {
      return o.__proto__ || Object.getPrototypeOf(o);
    };
    return _getPrototypeOf(o);
  }

  function _isNativeFunction(fn) {
    return Function.toString.call(fn).indexOf("[native code]") !== -1;
  }

  function _isNativeReflectConstruct() {
    if (typeof Reflect === "undefined" || !Reflect.construct) return false;
    if (Reflect.construct.sham) return false;
    if (typeof Proxy === "function") return true;
    try {
      Boolean.prototype.valueOf.call(Reflect.construct(Boolean, [], function () {}));
      return true;
    } catch (e) {
      return false;
    }
  }

  function _construct(Parent, args, Class) {
    if (_isNativeReflectConstruct()) {
      _construct = Reflect.construct.bind();
    } else {
      _construct = function _construct(Parent, args, Class) {
        var a = [null];
        a.push.apply(a, args);
        var Constructor = Function.bind.apply(Parent, a);
        var instance = new Constructor();
        if (Class) _setPrototypeOf(instance, Class.prototype);
        return instance;
      };
    }
    return _construct.apply(null, arguments);
  }

  function _wrapNativeSuper(Class) {
    var _cache = typeof Map === "function" ? new Map() : undefined;
    _wrapNativeSuper = function _wrapNativeSuper(Class) {
      if (Class === null || !_isNativeFunction(Class)) return Class;
      if (typeof Class !== "function") {
        throw new TypeError("Super expression must either be null or a function");
      }
      if (typeof _cache !== "undefined") {
        if (_cache.has(Class)) return _cache.get(Class);
        _cache.set(Class, Wrapper);
      }
      function Wrapper() {
        return _construct(Class, arguments, _getPrototypeOf(this).constructor);
      }
      Wrapper.prototype = Object.create(Class.prototype, {
        constructor: {
          value: Wrapper,
          enumerable: false,
          writable: true,
          configurable: true
        }
      });
      return _setPrototypeOf(Wrapper, Class);
    };
    return _wrapNativeSuper(Class);
  }

  /**
   * Class selection list option
   */
  let SelectionListOption = /*#__PURE__*/function () {
    /**
     * Constructor
     * @param {*} text Inner HTML
     * @param {*} value Value
     * @param {*} defaultSelected Is selected
     */
    function SelectionListOption(text, value, defaultSelected) {
      _defineProperty(this, "_selected", undefined);
      _defineProperty(this, "_selectionList", undefined);
      _defineProperty(this, "_innerHTML", "");
      _defineProperty(this, "_value", "");
      _defineProperty(this, "_defaultSelected", false);
      this._innerHTML = String(text);
      this._value = String(value);
      this._defaultSelected = !!defaultSelected;
    }

    /**
     * Get text
     */
    _createClass(SelectionListOption, [{
      key: "text",
      get: function () {
        return this._innerHTML;
      }

      /**
       * Set text
       */,
      set: function (value) {
        this._innerHTML = String(value);
      }

      /**
       * Get inner HTML
       */
    }, {
      key: "innerHTML",
      get: function () {
        return this._innerHTML;
      }

      /**
       * Set inner HTML
       */,
      set: function (value) {
        this._innerHTML = value;
      }

      /**
       * Get value
       */
    }, {
      key: "value",
      get: function () {
        return this._value;
      }

      /**
       * Set value
       */,
      set: function (value) {
        this._value = String(value);
      }

      /**
       * Get default selected
       */
    }, {
      key: "defaultSelected",
      get: function () {
        return this._defaultSelected;
      }

      /**
       * Set default selected
       */,
      set: function (value) {
        this._defaultSelected = !!value;
      }

      /**
       * Get selection list
       */
    }, {
      key: "selectionList",
      get: function () {
        return this._selectionList;
      }

      /**
       * Set selection list
       */,
      set: function (value) {
        this._selectionList = value;
      }

      /**
       * Get selected
       */
    }, {
      key: "selected",
      get: function () {
        var _this$_selected;
        return (_this$_selected = this._selected) != null ? _this$_selected : this.defaultSelected;
      }

      /**
       * Set selected
       */,
      set: function (value) {
        var _this$_selectionList;
        this._selected = value;
        (_this$_selectionList = this._selectionList) == null ? void 0 : _this$_selectionList.render();
      }
    }]);
    return SelectionListOption;
  }();

  /**
   * Class Dynamic Selection List
   */
  let SelectionList = /*#__PURE__*/function (_HTMLElement) {
    _inheritsLoose(SelectionList, _HTMLElement);
    /**
     * Constructor
     */
    function SelectionList() {
      var _this;
      _this = _HTMLElement.call(this) || this;
      _defineProperty(_assertThisInitialized(_this), "containerClass", "d-sm-table");
      _defineProperty(_assertThisInitialized(_this), "rowClass", "d-sm-table-row");
      _defineProperty(_assertThisInitialized(_this), "cellClass", "d-sm-table-cell");
      _defineProperty(_assertThisInitialized(_this), "options", []);
      _this._internals = _this.attachInternals();
      return _this;
    }

    /**
     * Connected
     */
    var _proto = SelectionList.prototype;
    _proto.connectedCallback = function connectedCallback() {
      let value = this.getAttribute("value") || "",
        values = this.multiple ? value.split(ew.MULTIPLE_OPTION_SEPARATOR) : [value];
      for (let val of values) this.add(val, "", true);
    }

    /**
     * Target element id
     */;
    /**
     * Add an option
     */
    _proto.add = function add(value, text, selected) {
      let option = new SelectionListOption(text, value, selected);
      this.addOption(option);
    }

    /**
     * Add an option
     */;
    _proto.addOption = function addOption(option) {
      if (option instanceof SelectionListOption) {
        option.selectionList = this;
        let index = this.options.findIndex(opt => opt.value == option.value);
        if (index > -1) this.options[index] = option;else this.options.push(option);
      }
    }

    /**
     * Remove an option
     */;
    _proto.remove = function remove(index) {
      let option = this.options[index];
      if (option) this.options.splice(index, 1);
    }

    /**
     * Remove all options
     */;
    _proto.removeAll = function removeAll() {
      this.options.splice(0);
    }

    /**
     * Clear selection
     */;
    _proto.clear = function clear() {
      for (let option of this.options) option.selected = false;
      this.render();
    }

    /**
     * Get random number
     */;
    _proto.getRandom = function getRandom() {
      return Math.floor(Math.random() * (999999 - 100000)) + 100000;
    }

    /**
     * Trigger change event
     */;
    _proto.triggerChange = function triggerChange() {
      const event = new Event("change", {
        view: window,
        bubbles: true,
        cancelable: false
      });
      this.dispatchEvent(event);
    }

    /**
     * Check if invalid
     */;
    _proto.isInvalid = function isInvalid(className) {
      return /\bis-invalid\b/.test(className);
    }

    /**
     * Check class
     */;
    _proto.attributeChangedCallback = function attributeChangedCallback(name, oldValue, newValue) {
      if (name == "class") {
        if (this.targetId && this.isInvalid(oldValue) != this.isInvalid(newValue)) {
          // "is-invalid" toggled
          let target = document.getElementById(this.targetId),
            inputs = target.querySelectorAll("input"),
            isInvalid = this.isInvalid(newValue);
          inputs.forEach(input => input.classList.toggle("is-invalid", isInvalid));
        }
      } else if (name == "value") {
        this._internals.setFormValue(this.value);
        if (this.value !== "") this.classList.remove("is-invalid");
      }
    }

    /**
     * Show loading
     */;
    _proto.showLoading = function showLoading() {
      var _this$target;
      (_this$target = this.target) == null ? void 0 : _this$target.appendChild(document.createRange().createContextualFragment(ew.spinnerTemplate()));
    }

    /**
     * Hide loading
     */;
    _proto.hideLoading = function hideLoading() {
      var _this$target2, _this$target2$querySe;
      (_this$target2 = this.target) == null ? void 0 : (_this$target2$querySe = _this$target2.querySelector(".ew-loading")) == null ? void 0 : _this$target2$querySe.remove();
    }

    /**
     * Render checkbox or radio in the target element
     */;
    _proto.render = function render() {
      let target = this.target,
        template = this.template;
      if (!target || !template || !this.list) return;

      // Clear the target
      while (target.firstChild) target.removeChild(target.firstChild);

      // Render
      target.innerHTML = ew.spinnerTemplate();
      let self = this,
        content = template.content,
        cols = this.columns || 1,
        tbl = document.createElement("div"),
        cnt = this.length,
        radioSuffix = "_" + this.getRandom(),
        isInvalid = this.classList.contains("is-invalid"),
        row;
      tbl.className = this.containerClass + " ew-item-container";
      let options = this.options.filter(opt => opt.value);
      options.forEach((option, i) => {
        let clone = content.cloneNode(true),
          input = clone.querySelector("input"),
          label = clone.querySelector("label"),
          suffix = "_" + this.getRandom(); // Make sure the id is unique
        input.name = input.name + (input.type == "radio" ? radioSuffix : suffix);
        input.id = input.id + suffix;
        input.value = option.value;
        input.setAttribute("data-index", i);
        input.checked = option.selected;
        input.classList.add("ew-custom-option");
        if (isInvalid) input.classList.add("is-invalid");
        input.addEventListener("click", function () {
          let index = parseInt(this.getAttribute("data-index"), 10);
          if (self.type == "select-one") {
            for (let option of self.options) option.selected = false;
          }
          self.options[index].selected = this.checked;
          self.setAttribute("value", self.value);
          self.triggerChange();
        });
        label.innerHTML = option.text;
        label.htmlFor = input.id;
        let cell = document.createElement("div");
        cell.className = this.cellClass;
        cell.appendChild(clone);
        if (i % cols == 0) {
          row = document.createElement("div");
          row.className = this.rowClass;
        }
        row.append(cell);
        if (i % cols == cols - 1) {
          tbl.append(row);
        } else if (i == cnt - 1) {
          // Last
          for (let j = i % cols + 1; j < cols; j++) {
            let c = document.createElement("div");
            c.className = this.cellClass;
            row.append(c);
          }
          tbl.append(row);
        }
      });
      target.innerHTML = "";
      target.append(tbl);
      this.setAttribute("value", this.value);
    }

    /**
     * Set focus
     */;
    _proto.focus = function focus() {
      if (this.list) {
        var _this$target3, _this$target3$querySe;
        (_this$target3 = this.target) == null ? void 0 : (_this$target3$querySe = _this$target3.querySelector("input")) == null ? void 0 : _this$target3$querySe.focus();
      } else {
        _HTMLElement.prototype.focus.call(this);
      }
    };
    _createClass(SelectionList, [{
      key: "form",
      get:
      /**
       * Get form
       */
      function () {
        return this._internals.form;
      }

      /**
       * Get name
       */
    }, {
      key: "name",
      get: function () {
        return this.getAttribute("name");
      }
    }, {
      key: "targetId",
      get: function () {
        return this.getAttribute("data-target");
      }

      /**
       * Target
       */
    }, {
      key: "target",
      get: function () {
        return this.parentNode.querySelector("#" + this.targetId);
      }

      /**
       * Template id
       */
    }, {
      key: "templateId",
      get: function () {
        return this.getAttribute("data-template");
      }

      /**
       * Template
       */
    }, {
      key: "template",
      get: function () {
        return this.parentNode.querySelector("#" + this.templateId);
      }

      /**
       * Input element id (for AutoSuggest)
       */
    }, {
      key: "inputId",
      get: function () {
        return this.getAttribute("data-input");
      }

      /**
       * Input element (for AutoSuggest)
       */
    }, {
      key: "input",
      get: function () {
        return this.parentNode.querySelector("#" + this.inputId);
      }

      /**
       * Is list
       */
    }, {
      key: "list",
      get: function () {
        return this.options;
      }

      /**
       * Number of columns
       */
    }, {
      key: "columns",
      get: function () {
        var _ew;
        if ((_ew = ew) != null && _ew.IS_MOBILE) {
          return 1;
        } else {
          let cols = this.getAttribute("data-repeatcolumn");
          return cols ? parseInt(cols, 10) : 1;
        }
      }

      /**
       * Length
       */
    }, {
      key: "length",
      get: function () {
        return this.options.length;
      }

      /**
       * Get selected index
       */
    }, {
      key: "selectedIndex",
      get: function () {
        return this.options.findIndex(option => option.selected);
      }

      /**
       * Set selected index
       */,
      set: function (index) {
        let option = this.options[index];
        if (option) {
          this.options.forEach(option => option.selected = false);
          option.selected = true;
          this.render();
        }
      }

      /**
       * Type
       */
    }, {
      key: "type",
      get: function () {
        return this.getAttribute("data-type") || this.getAttribute("type");
      }

      /**
       * Multiple
       */
    }, {
      key: "multiple",
      get: function () {
        return this.type == "select-multiple";
      }

      /**
       * Get value
       * @returns {string}
       */
    }, {
      key: "value",
      get: function () {
        if (this.type == "select-one" || this.type == "select-multiple") {
          return this.values.join(ew.MULTIPLE_OPTION_SEPARATOR || ",");
        } else {
          return this.getAttribute("value");
        }
      }

      /**
       * Get value as array
       * @returns {string[]}
       */,
      set:
      /**
       * Set value
       * @param {string|string[]} val
       */
      function (val) {
        if (this.type == "select-one") {
          for (let option of this.options) option.selected = option.value == val;
        } else if (this.type == "select-multiple") {
          let ar;
          if (Array.isArray(val)) {
            // Array
            ar = val.map(v => v != null ? v : String(v));
          } else {
            var _val;
            // String
            (_val = val) != null ? _val : val = String(val);
            ar = val ? val.split(ew.MULTIPLE_OPTION_SEPARATOR || ",") : [];
          }
          for (let option of this.options) option.selected = ar.includes(String(option.value));
        } else {
          this.setAttribute("value", val);
        }
        this.render();
      }
    }, {
      key: "values",
      get: function () {
        if (this.type == "select-one" || this.type == "select-multiple") {
          return this.options.filter(option => option.selected).map(option => option.value);
        } else {
          let val = this.getAttribute("value");
          return val ? val.split(ew.MULTIPLE_OPTION_SEPARATOR || ",") : [];
        }
      }
    }], [{
      key: "observedAttributes",
      get:
      /**
       * Options
       * @type {SelectionListOption[]}
       */

      /**
       * Specify observed attributes so that attributeChangedCallback will work
       */
      function () {
        return ["class", "value"];
      }

      /**
       * Form associcated
       */
    }, {
      key: "formAssociated",
      get: function () {
        return true;
      }
    }]);
    return SelectionList;
  }( /*#__PURE__*/_wrapNativeSuper(HTMLElement));

  // AdminLTE
  loadjs.ready("adminlte", function () {
    let $ = jQuery;

    // Init Treeview/SidebarSearch after rendering menu
    $(window).off("load.lte.treeview").off("load.lte.sidebarsearch");
    loadjs.ready("templates", () => {
      document.querySelectorAll("[data-widget=treeview]").forEach(el => $(el).Treeview(null)); // Use null so _init() will not be called again
      document.querySelectorAll("[data-widget=sidebar-search]").forEach(el => $(el).SidebarSearch(null)); // Use null so _init() will not be called again
    });

    // Card Refresh
    adminlte.CardRefresh.overlayTemplate = ew.overlayTemplate();
  });

  // loadjs
  window.loadjs = loadjs;

  // Define SelectionList
  customElements.define("selection-list", SelectionList);

  // Select2 templateResult/templateSelection
  let noop = () => {},
    templateCallback = o => (o.element instanceof HTMLOptionElement ? o.element.innerHTML : "") || o.text;
  let ew$1 = {
    SelectionListOption: SelectionListOption,
    PAGE_ID: "",
    // Page ID // To be updated in page
    RELATIVE_PATH: "",
    // Relative path // To be updated in page
    MULTIPLE_OPTION_SEPARATOR: ",",
    GENERATE_PASSWORD_UPPERCASE: true,
    GENERATE_PASSWORD_LOWERCASE: true,
    GENERATE_PASSWORD_NUMBER: true,
    GENERATE_PASSWORD_SPECIALCHARS: 1,
    // Max. number of special characters (number|boolean)
    CONFIRM_CANCEL: true,
    UNFORMAT_YEAR: 50,
    LAZY_LOAD_DELAY: 0,
    AJAX_DELAY: 5,
    LOOKUP_DELAY: 250,
    MAX_OPTION_COUNT: 3,
    NUMBERING_SYSTEM: "",
    Language: Language,
    // Class
    language: new Language(),
    // Language object
    vars: null,
    maps: {},
    addOptionDialog: null,
    emailDialog: null,
    importDialog: null,
    modalDialog: null,
    modalLookupDialog: null,
    ajaxBatchSize: 5,
    autoSuggestSettings: {
      highlight: true,
      hint: true,
      minLength: 1,
      debounce: 250,
      delay: 250,
      // For loading more results
      templates: {},
      // Custom templates for Typeahead (notFound, pending, header, footer, suggestion)
      classNames: {
        menu: "tt-menu dropdown-menu",
        input: 'tt-input form-control',
        dataset: "tt-dataset",
        suggestion: "tt-suggestion dropdown-item",
        cursor: "active"
      }
    },
    lightboxSettings: {
      transition: "none",
      photo: true,
      opacity: 0.5
    },
    calendarOptions: {
      showViewPageOnEventClick: false,
      useContextMenu: true,
      usePopover: true,
      fullCalendarOptions: {
        headerToolbar: {
          left: "prev,next today",
          center: "title",
          right: "dayGridMonth,timeGridWeek,timeGridDay,listWeek"
        },
        themeSystem: "bootstrap",
        // Bootstrap theme with Font Awesome icons
        initialView: "dayGridMonth",
        dayMaxEventRows: true,
        timeZone: "UTC",
        droppable: true,
        editable: true
      },
      eventPopoverOptions: {
        placement: "auto",
        trigger: "hover",
        // Either "hover" or "click"
        html: true
      }
    },
    uploadOptions: {
      // See https://github.com/blueimp/jQuery-File-Upload/wiki/Options
      cropperOptions: {
        // See https://github.com/fengyuanchen/cropperjs/blob/e969348d313dafe3416926125b21388cc67cefb1/README.md#options
        autoCropArea: 1,
        // Define the automatic cropping area size (between 0 and 1)
        viewMode: 2 // Restrict the minimum canvas size to fit within the container
      },

      cropperCanvasOptions: {
        // See https://github.com/fengyuanchen/cropperjs/blob/e969348d313dafe3416926125b21388cc67cefb1/README.md#getcroppedcanvasoptions
        minWidth: 256,
        minHeight: 256,
        maxWidth: 4096,
        maxHeight: 4096,
        fillColor: "#fff",
        imageSmoothingQuality: "high" // Quality of image smoothing
      }
    },

    importUploadOptions: {
      maxFileSize: 10000000,
      maxNumberOfFiles: 10
    },
    sweetAlertSettings: {
      showClass: {
        popup: "swal2-noanimation",
        // Disable popup animation
        backdrop: "swal2-noanimation",
        // Disable backdrop animation
        icon: "" // Disable icon animation
      },

      hideClass: {
        popup: "",
        // Disable popup animation
        backdrop: "",
        // Disable backdrop animation
        icon: "" // Disable icon animation
      },

      customClass: {
        container: "ew-swal2-container",
        popup: "ew-swal2-popup",
        header: "ew-swal2-header",
        title: "ew-swal2-title",
        closeButton: "ew-swal2-close-button",
        icon: "ew-swal2-icon",
        image: "ew-swal2-image",
        htmlContainer: "ew-swal2-html-container",
        content: "ew-swal2-content",
        input: "form-control ew-swal2-input",
        inputLabel: "ew-swal2-input-label",
        validationMessage: "ew-swal2-validation-message",
        actions: "ew-swal2-actions",
        confirmButton: "btn btn-primary ew-swal2-confirm-button",
        denyButton: "btn btn-danger ew-swal2-deny-button",
        cancelButton: "btn btn-secondary ew-swal2-cancel-button",
        loader: "ew-swal2-loader",
        footer: "ew-swal2-footer"
      }
    },
    selectOptions: {
      // Select2 options
      allowClear: true,
      theme: "bootstrap5",
      width: "style",
      minimumResultsForSearch: 20,
      escapeMarkup: v => v,
      templateResult: templateCallback,
      templateSelection: templateCallback,
      // Custom options
      debounce: 250,
      // For ajax.delay, see https://select2.org/data-sources/ajax#rate-limiting-requests
      customOption: true,
      containerClass: "d-sm-table",
      rowClass: "d-sm-table-row",
      cellClass: "d-sm-table-cell text-nowrap",
      iconClass: "form-check-label"
    },
    selectMinimumInputLength: 1,
    modalLookupOptions: {
      // Select2 options
      allowClear: true,
      theme: "bootstrap5",
      width: "100%",
      escapeMarkup: v => v,
      templateResult: templateCallback,
      templateSelection: templateCallback,
      closeOnSelect: false,
      minimumInputLength: 0,
      dropdownCssClass: "ew-modal-dropdown",
      // Custom options
      modal: true,
      debounce: 250,
      // For ajax.delay, see https://select2.org/data-sources/ajax#rate-limiting-requests
      draggableOptions: {
        cursor: "grabbing"
      } // See https://api.jqueryui.com/draggable/
    },

    filterOptions: {
      // Select2 options
      allowClear: true,
      theme: "bootstrap5",
      width: "100%",
      escapeMarkup: v => v,
      templateResult: templateCallback,
      templateSelection: templateCallback,
      closeOnSelect: false,
      minimumInputLength: 0,
      dropdownAutoWidth: true,
      dropdownCssClass: "ew-filter-dropdown",
      // Custom options
      debounce: 250,
      // For ajax.delay, see https://select2.org/data-sources/ajax#rate-limiting-requests
      customOption: true,
      columns: 1,
      multiple: true,
      preventScroll: true,
      // For focusing the search box
      containerClass: "container",
      rowClass: "row",
      cellClass: "col text-nowrap",
      iconClass: "form-check-label"
    },
    importTabulatorOptions: {
      height: 300
    },
    // See http://tabulator.info/docs/5.1/options
    toastOptions: {
      position: "topRight" // topRight|topLeft|bottomRight|bottomLeft
    },

    DOMPurifyConfig: {},
    sanitize: function (str) {
      return DOMPurify.sanitize(str, this.DOMPurifyConfig);
    },
    overlayScrollbarsOptions: {
      // Overlay scrollbars options
      className: "os-theme-dark",
      sizeAutoCapable: true,
      scrollbars: {
        autoHide: "leave",
        clickScrolling: true
      }
    },
    draggableOptions: {
      cursor: "grabbing"
    },
    // See https://api.jqueryui.com/draggable/
    queryBuilderOptions: {
      allowViewRules: false,
      // Show view rules button
      allowClearRules: false // Show clear rules button
    },

    // Query builder options
    queryBuilderPlugins: {
      // Query builder plugins
      "filter-description": null,
      "unique-filter": null,
      // "invert": null,
      "not-group": null
    },
    queryBuilderErrorClass: "invalid-tooltip",
    // "invalid-tooltip" or "invalid-feedback"
    bundleIds: ["dom", "head"],
    // All bundle IDs
    tooltipOptions: {
      placement: "bottom",
      customClass: "ew-custom-tooltip",
      sanitizeFn: null
    },
    popoverOptions: {
      html: true,
      customClass: "ew-custom-popover",
      sanitizeFn: null
    },
    PDFObjectOptions: {},
    chartConfig: {},
    spinnerClass: "spinner-border text-primary",
    // spinner-border or spinner-grow
    screenMediaQuery: "(min-width: 768px)",
    // Media query string for minimum screen width (not mobile), should matches $screen-sm-min
    jsRenderHelpers: {},
    // JsRender helpers
    jsRenderAttributes: ["src", "href", "title"],
    // Attributes supporting built-in JsRender tags
    autoHideSuccessMessage: true,
    autoHideSuccessMessageDelay: 5000,
    searchOperatorChange: noop,
    setLanguage: noop,
    addOptionDialogShow: noop,
    importDialogShow: noop,
    toggleSearchOperator: noop,
    togglePassword: noop,
    sort: noop,
    selectKey: noop,
    export: noop,
    exportWithCharts: noop,
    setSearchType: noop,
    emailDialogShow: noop,
    selectAll: noop,
    selectAllKeys: noop,
    submitAction: noop,
    addGridRow: noop,
    confirmDelete: () => false,
    deleteGridRow: () => false
  };

  /**
   * Init panel/header/footer
   *
   * @param {HTMLElement} el - Element
   */
  ew$1.initPanel = el => {
    if (el.dataset.isset) return;
    let html = "";
    for (let child of el.children) {
      html = child.innerHTML.trim();
      if (html !== "") break;
    }
    if (html === "") el.classList.add("d-none");
    el.dataset.isset = true;
  };

  // Panel selectors
  ew$1.panelSelectors = [".user-panel", ".ew-grid-upper-panel", ".ew-grid-lower-panel", ".ew-multi-column-row > * > .card > .card-header", ".ew-multi-column-row > * > .card > .card-footer"];

  /**
   * Init panels/headers/footers
   */
  ew$1.initPanels = el => el == null ? void 0 : el.querySelectorAll(ew$1.panelSelectors.join(",")).forEach(ew$1.initPanel);

  // Request animation frame to init panels
  let _initPanelsReq;
  let _initPanels = () => {
    ew$1.initPanels(document);
    _initPanelsReq = requestAnimationFrame(_initPanels);
  };
  loadjs.ready("wrapper", () => _initPanelsReq = requestAnimationFrame(_initPanels));

  // DOM content loaded
  document.addEventListener("DOMContentLoaded", () => {
    ew$1.initPanels(document);
    cancelAnimationFrame(_initPanelsReq);
    loadjs.done("dom");
  });

  /**
   * Initiate script load (async in series) and register bundle
   * @param {(string|string[])} paths - The file paths
   * @param {(string|Function|Object)} [arg1] - The (1) bundleId or (2) success
   *   callback or (3) object literal with success/error arguments, numRetries,
   *   etc.
   * @param {(Function|Object)} [arg2] - The (1) success callback or (2) object
   *   literal with success/error arguments, numRetries, etc.
   */
  ew$1.loadjs = function (paths, arg1, arg2) {
    let bundleId = arg1 != null && arg1.trim ? arg1 : "";
    if (bundleId && bundleId != "load" && !ew$1.bundleIds.includes(bundleId)) ew$1.bundleIds.push(bundleId);
    let args = (bundleId ? arg2 : arg1) || {};
    paths = Array.isArray(paths) ? paths : [paths];
    paths = paths.filter(path => path && (!Array.isArray(path) || path.length)); // Valid paths
    if (args.call)
      // Accept function as argument
      args = {
        success: args
      };
    args = {
      ...args,
      returnPromise: true
    };
    let clone = {
        ...args
      },
      p = Promise.resolve();
    delete clone.success;
    paths.forEach((path, i, ar) => {
      if (i == ar.length - 1)
        // Last
        p = p.then(() => loadjs(path, bundleId || args, bundleId ? args : null).catch(paths => console.log(paths)));else p = p.then(() => loadjs(path, clone).catch(paths => console.log(paths)));
    });
    return p;
  };

  /**
   * Initiate script load (async in series) when dependencies have been satisfied
   * @param {(string|string[])} deps - List of bundle ids
   * @param {(string|string[])} paths - The file paths
   * @param {(string|Function|Object)} [arg1] - The (1) bundleId or (2) success
   *   callback or (3) object literal with success/error arguments, numRetries,
   *   etc.
   * @param {(Function|Object)} [arg2] - The (1) success callback or (2) object
   *   literal with success/error arguments, numRetries, etc.
   */
  ew$1.ready = function (deps, paths, arg1, arg2) {
    let bundleId = arg1 != null && arg1.trim ? arg1 : "";
    if (bundleId && bundleId != "load" && !ew$1.bundleIds.includes(bundleId)) ew$1.bundleIds.push(bundleId);
    loadjs.ready(deps, function () {
      ew$1.loadjs(paths, arg1, arg2);
    });
  };

  // Global client script
  loadjs.ready("head", function () {
    ew$1.clientScript();
  });

  // Global startup script
  loadjs.ready("foot", function () {
    ew$1.startupScript();
    loadjs.done("load");
  });

  // Deep merge two or more objects into the first object
  let deepAssign = function () {
    let extended = {},
      overwrite = true,
      i = 0;
    // Check if overwrite
    if (arguments[0] === false) {
      // First argument is boolean
      overwrite = false;
      i++;
    }
    if (typeof arguments[i] == "object") {
      // Get first object
      extended = arguments[i];
      i++;
    }
    // Merge the object into the extended object
    let merge = function (obj) {
      for (let prop in obj) {
        if (obj.hasOwnProperty(prop)) {
          // If property is an object, merge properties
          if (typeof obj[prop] == "object" && !Array.isArray(obj[prop]) && obj[prop] !== null) {
            // Note: Type of array/null is "object"
            extended[prop] = deepAssign(overwrite, extended[prop], obj[prop]);
          } else {
            if (overwrite) {
              extended[prop] = obj[prop];
            } else {
              if (!extended.hasOwnProperty(prop)) extended[prop] = obj[prop];
            }
          }
        }
      }
    };
    // Loop through each object and merge
    for (; i < arguments.length; i++) {
      if (typeof arguments[i] == "object") merge(arguments[i]);
    }
    return extended;
  };
  ew$1.deepAssign = deepAssign;

  /**
   * Bundle IDs for applying client side template
   */
  ew$1.applyTemplateId = ["jsrender", "makerjs"];

  /**
   * Render client side template, use the HTML in DOM and return the HTML
   *
   * @param {jQuery} tmpl - Template
   * @param {Object} data - Data
   * @returns HTML string
   */
  ew$1.renderTemplate = function (tmpl, data) {
    let $ = jQuery,
      $tmpl = tmpl != null && tmpl.render ? tmpl : $(tmpl);
    if (!$tmpl.render) return;
    let args = {
      $template: $tmpl,
      data: data
    };
    $(document).trigger("rendertemplate", [args]);
    let html = $tmpl.render(args.data, ew$1.jsRenderHelpers),
      method = args.$template.data("method"),
      target = args.$template.data("target");
    if (html && method && target)
      // Render by specified method to target
      $(html)[method](target);else if (html && !method && target)
      // No method, render as inner HTML of target
      $(target).html(html);else if (html && !method && !target)
      // No method and target, render locally
      $tmpl.parent().append(html);
    loadjs.done("template." + $tmpl.data("name"));
    return html;
  };

  /**
   * Render all client side templates
   *
   * @param {Event} e - Event
   */
  ew$1.renderJsTemplates = function (e) {
    var _e$target;
    let $ = jQuery,
      el = (_e$target = e == null ? void 0 : e.target) != null ? _e$target : document;
    Array.from(el.querySelectorAll(".ew-js-template")).sort((a, b) => {
      a = parseInt(a.dataset.seq, 10) || 0;
      b = parseInt(b.dataset.seq, 10) || 0;
      return a - b;
    }).forEach(tmpl => {
      let name = tmpl.dataset.name,
        data = tmpl.dataset.data;
      if (data && typeof data == "string") {
        data = ew$1.vars[data] || window[data]; // Get data from ew.vars or global
        if (!data)
          // Data not found
          return;
      }
      if (name) {
        if (!$.render[name]) {
          // Render the first template of any named template only
          $.templates(name, tmpl.text);
          ew$1.renderTemplate($(tmpl), data);
        }
      } else {
        ew$1.renderTemplate($(tmpl), data);
      }
    });
    loadjs.done("templates");
  };

  // Overlay template
  ew$1.overlayTemplate = function () {
    return '<div class="overlay"><div class="' + this.spinnerClass + '" style="width: 3rem; height: 3rem;" role="status"><span class="visually-hidden">' + this.language.phrase("Loading") + '</span></div></div>';
  };

  // Spinner template
  ew$1.spinnerTemplate = function () {
    return '<div class="' + this.spinnerClass + ' m-3 ew-loading" role="status"><span class="visually-hidden">' + this.language.phrase("Loading") + '</span></div>';
  };

  return ew$1;

})();
