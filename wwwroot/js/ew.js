/*!
 * JavaScript for ASP.NET
 */
(function (ew$1, $$1, luxon) {
  'use strict';

  var _interopDefaultLegacy = e => e && typeof e === 'object' && 'default' in e ? e : { default: e };

  var ew__default = /*#__PURE__*/_interopDefaultLegacy(ew$1);
  var $__default = /*#__PURE__*/_interopDefaultLegacy($$1);
  var luxon__default = /*#__PURE__*/_interopDefaultLegacy(luxon);

  function MultiPage(formid) {
    var self = this;
    this.$form = null;
    this.formID = formid;
    this.pageIndex = 1;
    this.maxPageIndex = 0;
    this.minPageIndex = 0;
    this.pageIndexes = [];
    this.$pages = null;
    this.$collapses = null;
    this.isTab = false; // Is tabs
    this.isCollapse = false; // Is collapses (accordion)
    this.lastPageSubmit = false; // Enable submit button for the last page only
    this.hideDisabledButton = false; // Hide disabled submit button
    this.hideInactivePages = false; // Hide inactive pages
    this.lockTabs = false; // Set inactive tabs as disabled
    this.hideTabs = false; // Hide all tabs
    this.showPagerTop = false; // Show pager at top
    this.showPagerBottom = false; // Show pager at bottom
    this.pagerTemplate = '<nav><ul class="pagination"><li class="page-item previous ew-prev"><a href="#" class="page-link"><i class="fa-solid fa-angle-left"></i> {Prev}</a></li><li class="page-item next ew-next"><a href="#" class="page-link">{Next} <i class="fa-solid fa-angle-right"></i></a></li></ul></nav>'; // Pager template

    // "show" handler (for disabled tabs)
    var _show = function (e) {
      e.preventDefault();
    };

    // Set properties
    var _properties = ["lastPageSubmit", "hideDisabledButton", "hideInactivePages", "lockTabs", "hideTabs", "showPagerTop", "showPagerBottom", "pagerTemplate"];
    this.set = function () {
      if (arguments.length == 1 && $__default.default.isObject(arguments[0])) {
        var obj = arguments[0];
        for (var i in obj) {
          var p = i[0].toLowerCase() + i.substring(1); // Camel case
          if (_properties.includes(p)) this[p] = obj[i];
        }
      }
    };

    // DOM loaded
    this.init = function () {
      var tpl = this.pagerTemplate.replace(/\{prev\}/i, ew.language.phrase("Prev")).replace(/\{next\}/i, ew.language.phrase("Next"));
      if (this.isTab) {
        if (this.showPagerTop) this.$pages.closest(".ew-nav").before(tpl);
        if (this.showPagerBottom) this.$pages.closest(".ew-nav").after(tpl);
        this.$form.find(".ew-prev").click(function (e) {
          self.$pages.off("show.bs.tab", _show).filter(".active").parent().prev(":has([data-bs-toggle=tab]:not(.ew-hidden):not(.ew-disabled))").find("[data-bs-toggle=tab]").toggleClass("disabled d-none", false).click();
          return false;
        });
        this.$form.find(".ew-next").click(function (e) {
          self.$pages.off("show.bs.tab", _show).filter(".active").parent().next(":has([data-bs-toggle=tab]:not(.ew-hidden):not(.ew-disabled))").find("[data-bs-toggle=tab]").toggleClass("disabled d-none", false).click();
          return false;
        });
        if (this.hideTabs) this.$form.find(".ew-multi-page > .ew-nav > .nav-tabs").hide();
      } else if (this.isCollapse) {
        if (this.showPagerTop) this.$collapses.closest(".ew-accordion").before(tpl);
        if (this.showPagerBottom) this.$collapses.closest(".ew-accordion").after(tpl);
        this.$form.find(".ew-prev").click(function (e) {
          self.$pages.closest(".accordion-item").filter(":has(.collapse.show)").prev(":has([data-bs-toggle=collapse]:not(.ew-hidden):not(.ew-disabled))").toggleClass("disabled d-none", false).find("[data-bs-toggle=collapse]").click();
          return false;
        });
        this.$form.find(".ew-next").click(function (e) {
          self.$pages.closest(".accordion-item").filter(":has(.collapse.show)").next(":has([data-bs-toggle=collapse]:not(.ew-hidden):not(.ew-disabled))").toggleClass("disabled d-none", false).find("[data-bs-toggle=collapse]").click();
          return false;
        });
      }
      this.pageShow();
    };

    // Page show
    this.pageShow = function () {
      if (this.isTab) {
        if (this.lockTabs) this.$pages.on("show.bs.tab", _show);
        this.$pages.each(function () {
          var $this = $__default.default(this);
          if (self.hideInactivePages) $this.toggleClass("d-none", !$this.hasClass("active"));
          if (self.lockTabs) $this.toggleClass("disabled", !$this.hasClass("active"));
        });
      } else if (this.isCollapse) {
        this.$pages.closest(".accordion-item").each(function () {
          var $this = $__default.default(this);
          if (self.hideInactivePages) $this.toggleClass("d-none", !$this.find(".collapse.show")[0]);
        });
      }
      var disabled = this.lastPageSubmit && this.pageIndex != this.maxPageIndex;
      var $btn = this.$form.closest(".content, .modal-content").find("#btn-action, button.ew-submit").prop("disabled", disabled).toggle(!this.hideDisabledButton || !disabled);
      $__default.default(".ew-captcha").toggle($btn.is(":visible:not(:disabled)")); // Re-captcha uses class "disabled", not "disabled" property.
      disabled = this.pageIndex <= this.minPageIndex;
      this.$form.find(".ew-prev").toggleClass("disabled", disabled);
      disabled = this.pageIndex >= this.maxPageIndex;
      this.$form.find(".ew-next").toggleClass("disabled", disabled);
    };

    // Go to page by index
    this.gotoPage = function (i) {
      if (i <= 0 || i < this.minPageIndex || i > this.maxPageIndex) return;
      if (this.pageIndex != i) {
        var $page = this.$pages.eq(i - 1);
        if (this.isTab) {
          if ($page.is(":not(.d-none):not(.disabled)")) $page.click();else $page.parent().next(":has([data-bs-toggle=tab]):not(.d-none):not(.disabled)").find("[data-bs-toggle=tab]").toggleClass("disabled", false).click();
        } else if (this.isCollapse) {
          var $p = $page.closest(".accordion-item");
          if ($p.is(":not(.d-none)")) $page.click();else $p.next(":has([data-bs-toggle=collapse]):not(.d-none)").find("[data-bs-toggle=collapse]").click();
        }
        this.pageIndex = i;
      }
    };
    this.gotoPageByIndex = this.gotoPage;

    // Go to page by element
    this.gotoPageByElement = function (el) {
      this.gotoPage(parseInt($__default.default(el).data("page"), 10) || -1);
    };

    // Go to page by element's id or name or data-field attribute
    this.gotoPageByElementId = function (id) {
      var $el = this.$form.find("[data-page]").filter("[id='" + id + "'],[name='" + id + "'],[data-field='" + id + "']");
      this.gotoPageByElement($el);
    };

    // Toggle page
    this.togglePage = function (i, show) {
      if (this.isTab) {
        this.$pages.eq(i - 1).toggleClass("d-none", !show);
      } else if (this.isCollapse) {
        this.$pages.eq(i - 1).closest(".accordion-item").toggle("d-none", !show);
      }
    };

    // Render
    this.render = function () {
      this.$form = $__default.default("#" + formid);
      this.pageIndexes = this.$form.find("[data-page]").map(function () {
        var index = parseInt(this.dataset.page, 10);
        return index > 0 ? index : null;
      }).get();
      this.pageIndexes.sort(function (a, b) {
        return a - b;
      });
      this.minPageIndex = this.pageIndexes[0];
      this.maxPageIndex = this.pageIndexes[this.pageIndexes.length - 1];
      var $tabs = this.$form.find("[data-bs-toggle=tab]");
      if ($tabs[0]) {
        this.$pages = $tabs;
        this.isTab = true;
        $tabs.on("shown.bs.tab", function (e) {
          self.pageIndex = $tabs.index(e.target) + 1;
          self.pageShow();
          $__default.default($__default.default(this).attr("href")).find(".ew-map").each(function () {
            var m = ew.maps[this.id];
            if (m != null && m["map"]) {
              google.maps.event.trigger(m["map"], "resize");
              m["map"].setCenter(m["latlng"]);
            }
          });
        });
        this.pageIndex = $tabs.index($tabs.parent(".active")) + 1;
      } else {
        this.$collapses = this.$form.find("[data-bs-toggle=collapse]");
        if (this.$collapses[0]) {
          this.$pages = this.$collapses;
          this.isCollapse = true;
          var $bodies = this.$collapses;
          $bodies.on("shown.bs.collapse", function (e) {
            self.pageIndex = $bodies.index(e.target) + 1;
            self.pageShow();
            $__default.default(this).find(".ew-map").each(function () {
              var m = ew.maps[this.id];
              if (m != null && m["map"]) {
                google.maps.event.trigger(m["map"], "resize");
                m["map"].setCenter(m["latlng"]);
              }
            });
          });
          this.pageIndex = $bodies.index($bodies.hasClass("show")) + 1;
        }
      }
      $__default.default(function () {
        self.init();
      });
    };
  }

  let DateTime$1 = luxon__default.default.DateTime;

  /**
   * User level ID validator
   */
  function userLevelId(el) {
    if (el && !ew.checkInteger(el.value)) return {
      userLevelId: ew.language.phrase("UserLevelIDInteger")
    };
    let level = parseInt(el.value, 10);
    if (level < 1) return {
      userLevelId: ew.language.phrase("UserLevelIDIncorrect")
    };
    return false;
  }

  /**
   * User level name validator
   * @param {string} id User ID Field input element ID
   */
  function userLevelName(id) {
    return function (el) {
      let elId = document.getElementById("x_" + id);
      if (elId && el) {
        let name = el.value.trim(),
          level = parseInt(elId.value.trim(), 10);
        if (level === 0 && !ew.sameText(name, "Default")) {
          return {
            userLevelName: ew.language.phrase("UserLevelDefaultName")
          };
        } else if (level === -1 && !ew.sameText(name, "Administrator")) {
          return {
            userLevelName: ew.language.phrase("UserLevelAdministratorName")
          };
        } else if (level === -2 && !ew.sameText(name, "Anonymous")) {
          return {
            userLevelName: ew.language.phrase("UserLevelAnonymousName")
          };
        } else if (level > 0 && ["anonymous", "administrator", "default"].includes(name.toLowerCase())) {
          return {
            userLevelName: ew.language.phrase("UserLevelNameIncorrect")
          };
        }
      }
      return false;
    };
  }

  /**
   * Required validator
   */
  function required(fieldName) {
    return function (el) {
      var _$el$data;
      let $el = $__default.default(el),
        $p = $el.closest("#r_" + ((_$el$data = $el.data("field")) == null ? void 0 : _$el$data.substring(2))); // Find the row
      if (!$p[0]) $p = $el.closest("[id^=el]"); // Find the span
      if ($p.css("display") == "none") {
        // Hidden by .visible()
        return false;
      }
      if (el && !ew.hasValue(el)) {
        return {
          required: ew.language.phrase("EnterRequiredField").replace("%s", fieldName)
        };
      }
      return false;
    };
  }

  /**
   * File required validator
   */
  function fileRequired(fieldName) {
    return function (el) {
      let elFn = document.getElementById("fn_" + el.id);
      if (elFn && !ew.hasValue(elFn)) {
        return {
          fileRequired: ew.language.phrase("EnterRequiredField").replace("%s", fieldName)
        };
      }
      return false;
    };
  }

  /**
   * Mismatch password validator
   */
  function mismatchPassword(el) {
    let id;
    if (el.id.startsWith("c_"))
      // Confirm Password field in Register page
      id = el.id.replace(/^c_/, "x_");else if (el.id == "cpwd")
      // Change Password page
      id = "npwd";
    let elPwd = document.getElementById(id);
    if (el.value !== elPwd.value) {
      return {
        mismatchPassword: ew.language.phrase("MismatchPassword")
      };
    }
    return false;
  }

  /**
   * Between validator
   */
  function between(el) {
    let x, z;
    if (el.id.startsWith("y_")) {
      x = document.getElementById(el.id.replace(/^y_/, "x_"));
      z = document.getElementById(el.id.replace(/^y_/, "z_"));
    } else if (el.id.endsWith("_value_1")) {
      // QueryBuider
      x = document.getElementById(el.id.replace(/_value_1$/, "_value_0"));
      z = document.getElementById(el.id.replace(/_value_1$/, "_operator"));
    }
    if (ew.hasValue(x) && $__default.default(z).val().toUpperCase() == "BETWEEN" && !ew.hasValue(el)) {
      return {
        between: ew.language.phrase("EnterValue2")
      };
    }
    return false;
  }

  /**
   * Password strength validator
   */
  function passwordStrength(el) {
    let $el = $__default.default(el);
    if (!ew.isMaskedPassword(el) && $el.hasClass("ew-password-strength") && !$el.data("validated")) {
      return {
        passwordStrength: ew.language.phrase("PasswordTooSimple")
      };
    }
    return false;
  }

  /**
   * User name validator
   */
  function username(raw) {
    return function (el) {
      if (!raw && el.value.match(new RegExp('[' + ew.escapeRegExChars(ew.INVALID_USERNAME_CHARACTERS) + ']'))) return {
        username: ew.language.phrase("InvalidUsernameChars")
      };
      return false;
    };
  }

  /**
   * Password validator
   */
  function password(raw) {
    return function (el) {
      if (!raw && !ew.ENCRYPTED_PASSWORD && el.value.match(new RegExp('[' + ew.escapeRegExChars(ew.INVALID_PASSWORD_CHARACTERS) + ']'))) return {
        password: ew.language.phrase("InvalidPasswordChars")
      };
      return false;
    };
  }

  /**
   * Email validator
   */
  function email(el) {
    let value = ew.getValue(el);
    if (!ew.checkEmail(value)) {
      return {
        email: ew.language.phrase("IncorrectEmail")
      };
    }
    return false;
  }

  /**
   * Emails validator
   */
  function emails(cnt, err) {
    return function (el) {
      let value = ew.getValue(el);
      if (!ew.checkEmails(value, cnt)) {
        return {
          emails: err
        };
      }
      return false;
    };
  }

  /**
   * DateTime validator
   * @param {string} format DateTime format
   */
  function datetime(format) {
    return function (el) {
      let value = ew.getValue(el);
      if (!ew.checkDate(value, format)) {
        return {
          datetime: ew.language.phrase("IncorrectDate").replace(/%s/g, format)
        };
      }
      return false;
    };
  }

  /**
   * Time validator
   * @param {string} format Time format
   */
  function time(format) {
    return function (el) {
      let value = ew.getValue(el);
      if (!ew.checkTime(value, format)) {
        return {
          time: ew.language.phrase("IncorrectTime").replace(/%s/g, format)
        };
      }
      return false;
    };
  }

  /**
   * Float validator
   */
  function float(el) {
    let value = ew.getValue(el);
    if (!ew.checkNumber(value)) {
      return {
        float: ew.language.phrase("IncorrectFloat")
      };
    }
    return false;
  }

  /**
   * Range validator
   * @param {number} min Min value
   * @param {number} max Max value
   */
  function range(min, max) {
    return function (el) {
      let value = ew.getValue(el);
      if (!ew.checkRange(value, min, max)) {
        return {
          range: ew.language.phrase("IncorrectRange").replace("%1", min).replace("%2", max)
        };
      }
      return false;
    };
  }

  /**
   * Integer validator
   */
  function integer(el) {
    let value = ew.getValue(el);
    if (!ew.checkInteger(value)) {
      return {
        integer: ew.language.phrase("IncorrectInteger")
      };
    }
    return false;
  }

  /**
   * US phone validator
   */
  function phone(el) {
    let value = ew.getValue(el);
    if (!ew.checkPhone(value)) {
      return {
        phone: ew.language.phrase("IncorrectPhone")
      };
    }
    return false;
  }

  /**
   * US ZIP validator
   */
  function zip(el) {
    let value = ew.getValue(el);
    if (!ew.checkZip(value)) {
      return {
        zip: ew.language.phrase("IncorrectZip")
      };
    }
    return false;
  }

  /**
   * Credit card validator
   */
  function creditCard(el) {
    let value = ew.getValue(el);
    if (!ew.checkCreditCard(value)) {
      return {
        creditCard: ew.language.phrase("IncorrectCreditCard")
      };
    }
    return false;
  }

  /**
   * US SSN validator
   */
  function ssn(el) {
    let value = ew.getValue(el);
    if (!ew.checkSsn(value)) {
      return {
        ssn: ew.language.phrase("IncorrectSSN")
      };
    }
    return false;
  }

  /**
   * GUID validator
   */
  function guid(el) {
    let value = ew.getValue(el);
    if (!ew.checkGuid(value)) {
      return {
        guid: ew.language.phrase("IncorrectGUID")
      };
    }
    return false;
  }

  /**
   * Regular expression validator
   * @param {string} pattern Regular expression pattern
   */
  function regex(pattern) {
    return function (el) {
      let value = ew.getValue(el);
      if (!ew.checkByRegEx(value, pattern)) {
        return {
          regex: ew.language.phrase("IncorrectField")
        };
      }
      return false;
    };
  }

  /**
   * URL validator
   */
  function url(el) {
    let value = ew.getValue(el);
    if (!ew.checkUrl(value)) {
      return {
        url: ew.language.phrase("IncorrectUrl")
      };
    }
    return false;
  }

  /**
    * Custom validator
    * @param {Function} fn Function(value, ...args)
    * @param {...any} args Additional arguments for the function
    */
  function custom(fn) {
    for (var _len = arguments.length, args = new Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
      args[_key - 1] = arguments[_key];
    }
    return function (el) {
      if (typeof fn == "function") {
        let value = ew.getValue(el);
        if (fn(value, ...args)) return {
          custom: ew.language.phrase("IncorrectField")
        };
      }
      return false;
    };
  }

  /**
   * Captcha validator
   */
  function captcha(el) {
    if (el && !ew.hasValue(el)) {
      return {
        captcha: ew.language.phrase("EnterValidateCode")
      };
    }
    return false;
  }

  /**
   * reCaptcha validator
   */
  function recaptcha(el) {
    var _grecaptcha;
    if (el && !ew.hasValue(el) && ((_grecaptcha = grecaptcha) == null ? void 0 : _grecaptcha.getResponse(el.dataset.id)) === "") {
      return {
        recaptcha: ew.language.phrase("ClickReCaptcha")
      };
    }
    return false;
  }

  /**
   * Min date/time validator
   * @param {Function|HTMLElement|DateTime|Date} min Min date
   */
  function minDate(min) {
    return function (el) {
      let value = ew.getValue(el),
        format = el.dataset.formatPattern,
        minDateTime;
      min = typeof min == "function" ? min() : min;
      if (min instanceof HTMLElement) {
        let minValue = min.value,
          minFormat = min.dataset.formatPattern;
        if (min.value && min.dataset.formatPattern) minDateTime = ew.parseDate(minValue, minFormat);
      } else if (min instanceof Date) {
        minDateTime = DateTime$1.fromJSDate(min, {
          zone: "utc"
        });
      } else if (min instanceof DateTime$1) {
        minDateTime = min;
      }
      if (value && format && minDateTime) {
        let dateTime = ew.parseDate(value, format);
        if (dateTime < minDateTime) return {
          minDate: ew.language.phrase("IncorrectMinDate").replace("%s", ew.formatDateTime(minDateTime, ew.DATE_FORMAT + " " + ew.TIME_FORMAT))
        };
      }
      return false;
    };
  }

  /**
   * Max date/time validator
   * @param {Function|HTMLElement|DateTime|Date} max Max date
   */
  function maxDate(max) {
    return function (el) {
      let value = ew.getValue(el),
        format = el.dataset.formatPattern,
        maxDateTime;
      max = typeof max == "function" ? max() : max;
      if (max instanceof HTMLElement) {
        let maxValue = max.value,
          maxFormat = max.dataset.formatPattern;
        if (max.value && max.dataset.formatPattern) maxDateTime = ew.parseDate(maxValue, maxFormat);
      } else if (max instanceof Date) {
        maxDateTime = DateTime$1.fromJSDate(max, {
          zone: "utc"
        });
      } else if (max instanceof DateTime$1) {
        maxDateTime = max;
      }
      if (value && format && maxDateTime) {
        let dateTime = ew.parseDate(value, format);
        if (dateTime > maxDateTime) return {
          maxDate: ew.language.phrase("IncorrectMaxDate").replace("%s", ew.formatDateTime(maxDateTime, ew.DATE_FORMAT + " " + ew.TIME_FORMAT))
        };
      }
      return false;
    };
  }

  /**
   * Min validator
   * @param {Function|HTMLElement|String|Number} min Min value
   */
  function min(min) {
    return function (el) {
      let value = ew.parseNumber(ew.getValue(el)),
        minValue;
      min = typeof min == "function" ? min() : min;
      if (min instanceof HTMLElement) {
        minValue = ew.parseNumber(ew.getValue(min));
      } else if (typeof min == "string") {
        minValue = ew.parseNumber(min);
      } else if (typeof min == "number") {
        minValue = min;
      }
      if (typeof value == "number" && typeof minValue == "number") {
        if (value < minValue) return {
          min: ew.language.phrase("IncorrectMin").replace("%s", minValue)
        };
      }
      return false;
    };
  }

  /**
   * Max validator
   * @param {Function|HTMLElement|String|Number} max Max value
   */
  function max(max) {
    return function (el) {
      let value = ew.parseNumber(ew.getValue(el)),
        maxValue;
      max = typeof max == "function" ? max() : max;
      if (max instanceof HTMLElement) {
        maxValue = ew.parseNumber(ew.getValue(max));
      } else if (typeof max == "string") {
        maxValue = ew.parseNumber(max);
      } else if (typeof max == "number") {
        maxValue = max;
      }
      if (typeof value == "number" && typeof maxValue == "number") {
        if (value > maxValue) return {
          max: ew.language.phrase("IncorrectMax").replace("%s", maxValue)
        };
      }
      return false;
    };
  }

  var Validators = {
    __proto__: null,
    userLevelId,
    userLevelName,
    required,
    fileRequired,
    mismatchPassword,
    between,
    passwordStrength,
    username,
    password,
    email,
    emails,
    datetime,
    time,
    float,
    range,
    integer,
    phone,
    zip,
    creditCard,
    ssn,
    guid,
    regex,
    url,
    custom,
    captcha,
    recaptcha,
    minDate,
    maxDate,
    min,
    max
  };

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

  /*
   * Based on: jquery.batch v0.1.0
   * Copyright 2013, Matt Morgan (@mlmorg)
   * MIT license
   */

  // Global batch settings
  $__default.default.batchSettings = {
    type: 'POST',
    contentType: 'application/json',
    processData: false,
    dataType: 'json',
    toJSON: JSON.stringify,
    parse: data => data
  };

  // Setup method
  $__default.default.batchSetup = function (options) {
    return $__default.default.extend($__default.default.batchSettings, options);
  };

  // $.batch class
  // -------------

  var Batch = $__default.default.batch = function (func, options) {
    // Always instantiate a Batch class even if called without "new"
    if (!(this instanceof Batch)) {
      return new Batch(func, options);
    }

    // Shift arguments if func is an object
    if (typeof func === 'object') {
      options = func;
      func = undefined;
    }

    // Default options
    this.options = $__default.default.extend({}, $__default.default.batchSettings, options);

    // Find a parent batch object, if we're nested
    this.parent = $__default.default.ajaxSetup()._batch;

    // Requests storage
    this.requests = [];

    // Add any requests
    if (func) {
      this.add(func);
    }
    return this;
  };

  // Our methods
  $__default.default.extend(Batch.prototype, {
    // Method for adding requests to the batch
    add: function (func) {
      var _func$name;
      // Set global _batch variable in jQuery.ajaxSettings
      $__default.default.ajaxSetup({
        _batch: this.parent || this
      });

      // Call the user's function
      if ((_func$name = func.name) != null && _func$name.startsWith('bound ')) {
        // Bound function
        func();
      } else {
        func.call($__default.default.ajaxSetup()._batch);
      }

      // Remove the global _batch variable when we're not nested
      if (!this.parent) {
        $__default.default.ajaxSetup({
          _batch: null
        });
      }
      return this;
    },
    // Clear requests storage
    clear: function () {
      this.requests = [];
    },
    // Method for running the batch request
    send: function (options) {
      options = options || {};
      var instance = this;

      // When we're handling a child batch object, wrap any success functions
      // and add them to the parent batch success
      if (this.parent && options.success) {
        var parentSuccess = this.parent.options.success;
        this.parent.options.success = function (data, status, xhr) {
          options.success(data, status, xhr);
          if (parentSuccess) {
            parentSuccess(data, status, xhr);
          }
        };
      }

      // When we're handling the top-most batch, send the request
      else if (this.requests.length) {
        // Map an array of requests
        var requests = $__default.default.map(this.requests, function (request) {
          return request.settings.data;
        });

        // Override the success callback
        var success = options.success;
        var childSuccess = this.options.success;
        options.success = function (data, statusText, xhr) {
          // Call our _deliver method to handle each individual batch request response
          instance._deliver.call(instance, data, statusText);

          // Child batch success functions
          if (childSuccess) {
            childSuccess(data, statusText, xhr);
          }

          // User's success function
          if (success) {
            success(data, statusText, xhr);
          }
        };

        // Build the Ajax request options
        options = $__default.default.extend({}, this.options, options);

        // Create hash of requests to pass as the data in the Ajax request
        if (!options.data) {
          options.data = $__default.default.batchSettings.toJSON(requests);
        }

        // Call the request
        return $__default.default.ajax(options);
      }
    },
    // Private method to add a request to the batch requests array
    _addRequest: function (xhr, settings) {
      this.requests.push({
        xhr: xhr,
        settings: settings
      });
    },
    // Delivers each batch request response to its intended xhr success/complete function
    _deliver: function (data, statusText) {
      var _responses$error;
      var instance = this;

      // Pass the response off to the user to parse out the responses
      var responses = $__default.default.batchSettings.parse(data);
      if (responses != null && (_responses$error = responses.error) != null && _responses$error.description) {
        var _responses$error2;
        ew__default.default.alert(responses == null ? void 0 : (_responses$error2 = responses.error) == null ? void 0 : _responses$error2.description);
        return;
      }

      // Loop through the responses
      $__default.default.each(responses, function (i, response) {
        var _request$settings$com;
        // Only work with batch requests that we have stored
        if (!instance.requests[i]) {
          return;
        }

        // Grab the stored request data
        var request = instance.requests[i];

        // Build statusText a la jQuery based on status code
        request.xhr.statusText = statusText;

        // Call the user/success function, if it exists. Pass the response body, status text and xhr.
        if (statusText === 'success') {
          var _request$settings$suc;
          (_request$settings$suc = request.settings.success) == null ? void 0 : _request$settings$suc.call(request.xhr, response, statusText);
        }

        // Call complete
        (_request$settings$com = request.settings.complete) == null ? void 0 : _request$settings$com.call(request.xhr, statusText);
      });
    }
  });

  // $.ajax override
  // ---------------

  // Override jQuery.ajax to cancel any outgoing requests called within
  // a $.batch() function and add them to the batch requests array for
  // that batch instance
  var $ajax = $__default.default.ajax;
  $__default.default.ajax = function (url, options) {
    // Shift arguments when options are passed as first argument
    if (typeof url === 'object') {
      options = url;
      url = undefined;
    }

    // Set options object
    options = options || {};

    // Override the jQuery beforeSend method
    var beforeSend = options.beforeSend;
    options.beforeSend = function (xhr, settings) {
      // Call the user's beforeSend function, if passed
      if (beforeSend) {
        var before = beforeSend(xhr, settings);

        // Cancel request if user's beforeSend function returns false
        if (before === false) {
          return before;
        }
      }

      // We're only worried about requests made within a $.batch function
      // (aka they have a _batch object)
      if (settings._batch) {
        // Add request to batch
        settings._batch._addRequest(xhr, settings);

        // Cancel this request
        return false;
      }
    };

    // Run original $.ajax method for all other requests
    return $ajax.call(this, url, options);
  };

  // Form class
  function FormBase(id, pageId) {
    let self = this,
      $self = $__default.default(self);
    this._initiated = false;
    this.id = id; // Same ID as the form
    this.element = document.getElementById(id); // HTML form or div
    this.$element = $__default.default(this.element); // jQuery object of the form or div
    this.pageId = pageId;
    this.htmlForm = null; // HTML form element
    this.initSearchPanel = false; // Expanded by default
    this.modified = false;
    this.emptyRow = null; // Check empty row
    this.multiPage = null; // Multi-page
    this.autoSuggests = {}; // AutoSuggests
    this.lists = {}; // Dynamic selection lists
    this.batch = new Batch(); // For batch lookup
    this.formKeyCountName = ""; // For list/grid pages
    this.submitWithFetch = false; // Submit form with Fetch API and returns Promise
    this.enableOnInit = false; // Disable form during init
    this.filterList = null;

    // Disable form
    this.disableForm = function () {
      document.body.style.cursor = "wait";
      let form = this.getForm();
      $__default.default(form.elements).filter(":submit:not(.dropdown-toggle), .ew-submit").prop("disabled", true).addClass("disabled");
      this.trigger("disabled");
    };

    // Enable form
    this.enableForm = function () {
      let form = this.getForm(),
        $elements = $__default.default(form.elements);
      $elements.filter(".ew-disabled-element").removeClass("ew-disabled-element").prop("disabled", false);
      $elements.filter(".ew-enabled-element").removeClass("ew-enabled-element").prop("disabled", true);
      $elements.filter(":submit:not(.dropdown-toggle), .ew-submit").not(".ew-disabled") // Exclude submit buttons with .ew-disabled (disabled purposely)
      .prop("disabled", false).removeClass("disabled");
      this.trigger("enabled");
      document.body.style.cursor = "default";
    };

    // Append hidden element with form name
    this.appendHidden = function (el) {
      var form = this.getForm(),
        $form = $__default.default(form),
        $dp = $__default.default(el).closest(".ew-form"),
        name = $dp.attr("id") + "$" + el.name;
      if ($form.find("input:hidden[name='" + name + "']")[0])
        // Already appended
        return;
      var ar = $dp.find('[name="' + el.name + '"]').serializeArray();
      if (ar.length) {
        ar.forEach(function (o, i) {
          $__default.default('<input type="hidden" name="' + name + '">').val(o.value).appendTo($form);
        });
      } else {
        $__default.default('<input type="hidden" name="' + name + '">').val("").appendTo($form);
      }
    };

    // Can submit
    this.canSubmit = async function (e) {
      var _await$this$validate, _this$validate;
      var form = this.getForm(),
        $form = $__default.default(form);
      this.disableForm();
      this.updateTextArea();
      if (((_await$this$validate = await ((_this$validate = this.validate) == null ? void 0 : _this$validate.call(this, e))) != null ? _await$this$validate : true) && !$form.find(".is-invalid")[0]) {
        $form.find("input[name^=sv_], input[name^=p_], [name*=_query_builder_rule_], .ew-template input, .ew-custom-option") // Do not submit these values
        .prop("disabled", true).addClass("ew-disabled-element");
        $form.find("[data-readonly=1][disabled]").prop("disabled", false).addClass("ew-enabled-element"); // Submit readonly values
        var $dps = $form.find("input[name=detailpage]").map(function (i, el) {
          return $form.find("#" + el.value)[0];
        });
        if ($dps.length > 1) {
          // Multiple Master/Detail, check element names
          $dps.each(function (i, dp) {
            $__default.default(dp).find(":input").each(function (j, el) {
              if (/^(fn_)?(x|o)\d*_/.test(el.name)) {
                var $els = $dps.not(dp).find(":input[name='" + el.name + "']");
                if ($els.length) {
                  // Elements with same name found
                  self.appendHidden(el); // Append element with form name
                  $els.each(function () {
                    self.appendHidden(this); // Append elements with same name and form name
                  });
                }
              }
            });
          });
        }

        let args = {
            form: form,
            result: true
          },
          evt = $__default.default.Event("beforesubmit", {
            originalEvent: e
          });
        $form.trigger(evt, [args]);
        let result = await args.result; // Support Promise<boolean|Object>
        if (!evt.isDefaultPrevented() && (result === true || $__default.default.isObject(result) && result.value))
          // Support Swal.fire()
          return true;
      } else {
        this.enableForm();
      }
      return false;
    };

    // Submit
    this.submit = async function (e) {
      var _e$originalEvent, _e$originalEvent$subm;
      let form = this.getForm(),
        formAction = e == null ? void 0 : (_e$originalEvent = e.originalEvent) == null ? void 0 : (_e$originalEvent$subm = _e$originalEvent.submitter) == null ? void 0 : _e$originalEvent$subm.formAction;
      if (formAction) {
        formAction = ew.parseUrl(formAction).pathname;
        form.setAttribute("action", formAction);
      }
      if (await this.canSubmit(e)) {
        if (this.submitWithFetch) {
          let url = form.getAttribute("action"),
            method = form.method.toUpperCase(),
            body = $__default.default(form).serialize();
          return ew.fetch(url, {
            method,
            body
          }).finally(() => this.enableForm()); // Return Promise
        } else {
          form.submit();
        }
      } else {
        this.enableForm();
      }
    };

    // Get dynamic selection list by element name or id
    this.getList = function (name) {
      var _this$getElement, _this$getElement$data;
      if ($__default.default.isObject(name)) {
        // Object
        let obj = name;
        name = obj.name || obj.id;
        if (name.includes("query_builder_rule")) name = obj.dataset.field;
      }
      name = name.replace(/^(sv_)?[xy](\d*|\$rowindex\$)_|\[\]$/g, ""); // Remove element name prefix/suffix
      if (this.lists[name]) return this.lists[name];
      let field = (_this$getElement = this.getElement(name)) == null ? void 0 : (_this$getElement$data = _this$getElement.dataset.field) == null ? void 0 : _this$getElement$data.replace(/^x_|\[\]$/g, ""); // Remove element name prefix/suffix
      return field ? this.lists[field] : {};
    };

    // Compile templates
    this.compileTemplates = function () {
      let lists = Object.values(this.lists);
      for (let list of lists) {
        if (list.template && $__default.default.isString(list.template)) list.template = $__default.default.templates(list.template);
      }
    };

    // Get option template of dynamic selection list
    this.getOptionTemplate = function (list) {
      if (list.template && $__default.default.isString(list.template)) list.template = $__default.default.templates(list.template);
      return list.template;
    };

    // Get the HTML form element
    this.getForm = function () {
      if (!this.htmlForm) {
        var _this$element, _this$element2;
        if (((_this$element = this.element) == null ? void 0 : _this$element.tagName) == "FORM") {
          // HTML form
          this.htmlForm = this.element;
        } else if (((_this$element2 = this.element) == null ? void 0 : _this$element2.tagName) == "DIV") {
          // HTML div => Grid page
          this.htmlForm = this.element.closest("form");
        }
      }
      return this.htmlForm;
    };

    // Get form element as single element
    this.getElement = function (name) {
      return name ? ew.getElement(name, this.$element) : this.$element[0];
    };

    // Get form element(s) as single element or array of radio/checkbox
    this.getElements = function (name) {
      return ew.getElements(name, this.$element);
    };

    // Fix ID
    this.fixId = function (id, multiple, rowindex) {
      let t = "",
        ar = id.split(" ");
      if (ar.length > 1) {
        t = ar[0];
        rowindex = "";
        id = ar[1];
      }
      let prefix = $__default.default.isNumber(rowindex) ? "x" + rowindex + "_" : "x_"; // Add row index
      id = id.startsWith("x_") ? id.replace(/^x_/, prefix) // Field element name
      : prefix + id; // Field var
      if (multiple && !id.endsWith("[]"))
        // Add [] if select-multiple
        id += "[]";
      return t ? t + " " + id : id;
    };

    /**
     * Update a selection list
     * @param {string|HTMLElement} id - ID (Field param) or HTML element
     * @param {Object} list - List
     * @param {(null|undefined|number)} rowindex - Row index
     * @param {boolean} immediate - Send request immediately
     * @returns
     */
    this.updateList = function (id, list, rowindex, immediate) {
      var _list, _Object$entries$find;
      (_list = list) != null ? _list : list = $__default.default.isString(id) // Find the list if not provided
      ? (_Object$entries$find = Object.entries(this.lists).find(entry => entry[0] == id.replace(/^x_/, ""))) == null ? void 0 : _Object$entries$find[1] // String
      : this.getList(id); // HTMLElement
      if (list.template && $__default.default.isString(list.template))
        // Compile template in case updateLists() called before init()
        list.template = $__default.default.templates(list.template);
      let form = this.element,
        parents = list.parentFields.slice().map(parent => this.fixId(parent, false, rowindex)),
        // Clone and fix index
        ajax = list.ajax && !list.lookupOptions.length; // Has link table and no lookup cache
      id = $__default.default.isString(id) ? this.fixId(id, list.multiple, rowindex) : id;
      return !ajax || immediate ? ew.updateOptions.call(this, id, parents, ajax, false) // Non-Ajax (lookup cache or user values) or update immediately
      : [id, parents.map(parent => ew.getOptionValues(parent, form)), ajax, false]; // Ajax (async) => to be batch updated
    };

    /**
     * Update selection lists
     * @param {(null|undefined|number)} rowindex - Row index
     * @param {bool} [immediate] - Send request immediately
     * @returns
     */
    this.updateLists = function (rowindex, immediate) {
      var _form$querySelector;
      if (rowindex === null)
        // rowindex == $rowindex$ == null
        return;
      if (this.pageId == "grid" && !$__default.default.isNumber(rowindex) && !$__default.default.isUndefined(rowindex)) return;
      let form = this.getForm(); // Set up $element and htmlForm
      if ((form == null ? void 0 : (_form$querySelector = form.querySelector("input#confirm")) == null ? void 0 : _form$querySelector.value) == "confirm")
        // Confirm page
        return;
      let selector = Object.entries(this.lists).map(_ref => {
        let [id, list] = _ref;
        return "[name='" + this.fixId(id, list.multiple, rowindex) + "']";
      }).join();
      if (selector && this.element.querySelector(selector))
        // List found
        Object.entries(this.lists).map(_ref2 => {
          let [id, list] = _ref2;
          return this.updateList(id, list, rowindex);
        }) // Update each list
        .filter(result => Array.isArray(result)) // Get ajax requests for batch update
        .forEach(request => this.batch.add(ew.updateOptions.bind(this, ...request))); // Batch update async requests
      // Update the Ajax lists
      if (this.batch.requests.length) {
        if (rowindex === undefined || immediate) {
          // Called by form or update immediately (add blank row)
          let deferreds = [],
            batchSize = ew.ajaxBatchSize > 0 ? ew.ajaxBatchSize : 1;
          while (this.batch.requests.length > batchSize) {
            let b = new Batch();
            b.requests = this.batch.requests.splice(0, batchSize);
            deferreds.push(b.send({
              url: ew.getApiUrl(ew.API_LOOKUP_ACTION)
            }));
          }
          if (this.batch.requests.length > 0) deferreds.push(this.batch.send({
            url: ew.getApiUrl(ew.API_LOOKUP_ACTION)
          }));
          $__default.default.when(...deferreds).then(() => $__default.default(document).trigger("updatedone", [{
            source: self,
            target: form
          }])).fail(error => console.log(error)).always(() => this.batch.clear());
        }
      } else {
        $__default.default(document).trigger("updatedone", [{
          source: self,
          target: form
        }]);
      }
    };

    // Create AutoSuggest
    this.createAutoSuggest = function (settings) {
      var options = Object.assign({
        limit: ew.AUTO_SUGGEST_MAX_ENTRIES,
        form: this
      }, ew.autoSuggestSettings, settings); // Global settings + field specific settings
      self.autoSuggests[settings.id] = new ew.AutoSuggest(options);
    };

    // Init editors
    this.initEditors = function () {
      var form = this.getForm();
      $__default.default(form.elements).filter("textarea.editor").each(function (i, el) {
        var ed = $__default.default(el).data("editor");
        if (ed && !ed.active && !ed.name.includes("$rowindex$")) ed.create();
      });
    };

    // Update textareas
    this.updateTextArea = function (name) {
      var form = this.getForm();
      $__default.default(form.elements).filter("textarea.editor").each(function (i, el) {
        var ed = $__default.default(el).data("editor");
        if (!ed || name && ed.name != name) return true; // Continue
        ed.save();
        if (name) return false; // Break
      });
    };

    // Destroy editor(s)
    this.destroyEditor = function (name) {
      var form = this.getForm();
      $__default.default(form.elements).filter("textarea.editor").each(function (i, el) {
        var ed = $__default.default(el).data("editor");
        if (!ed || name && ed.name != name) return true; // Continue
        ed.destroy();
        if (name) return false; // Break
      });
    };

    // Show error message
    this.onError = function (el, msg) {
      return ew.onError(this, el, msg);
    };

    // Init file upload
    this.initUpload = function () {
      var form = this.getForm();
      $__default.default(form.elements).filter("input:file:not([name*='$rowindex$'])").each(function (index) {
        $__default.default.later(ew.AJAX_DELAY * index, null, ew.upload, this); // Delay a little in case of large number of upload fields
      });
    };

    // Set up filters
    this.setupFilters = function (e, filters) {
      let id = this.id,
        data = this.filterList ? this.filterList.data : null,
        $sf = $__default.default(".ew-save-filter[data-form=" + id + "]").toggleClass("disabled", !data),
        $df = $__default.default(".ew-delete-filter[data-form=" + id + "]").toggleClass("disabled", !filters.length).toggleClass("dropdown-toggle", !!filters.length),
        $delete = $df.parent("li").toggleClass("dropdown-submenu dropdown-hover", !!filters.length).toggleClass("disabled", !filters.length),
        $save = $sf.parent("li").toggleClass("disabled", !data);
      let saveFilters = function (id, filters) {
        if (ew.CLIENT_SEARCH_FILTER) {
          localStorage.setItem(ew.PROJECT_NAME + "_" + id + "_filters", JSON.stringify(filters));
        } else if (ew.SERVER_SEARCH_FILTER) {
          document.body.style.cursor = "wait";
          $__default.default.ajax(ew.currentPage(), {
            type: "POST",
            dataType: "json",
            data: {
              "ajax": "savefilters",
              "filters": JSON.stringify(filters)
            }
          }).done(function (result) {
            var _result$;
            if ((_result$ = result[0]) != null && _result$.success) self.filterList.filters = filters; // Save filters
          }).always(function () {
            document.body.style.cursor = "default";
          });
        }
      };
      $save.off("click.ew").on("click.ew", function (e) {
        // Save filter
        if ($save.hasClass("disabled")) return false;
        ew.prompt({
          input: "text",
          html: ew.language.phrase("EnterFilterName")
        }, name => {
          name = ew.sanitize(name);
          if (name) {
            filters.push([name, data]);
            saveFilters(id, filters);
          }
        }, true);
      }).prevAll().remove();
      $df.next("ul.dropdown-menu").remove();
      if (filters.length) {
        let $submenu = $__default.default('<ul class="dropdown-menu"></ul>');
        filters.forEach((filter, i, ar) => {
          $__default.default('<li><a class="dropdown-item" data-index="' + i + '" data-ew-action="none">' + filter[0] + '</a></li>').on("click", function (e) {
            // Delete
            let index = this.querySelector("a[data-index]").dataset.index;
            ew.prompt(ew.language.phrase("DeleteFilterConfirm").replace("%s", filter[0]), result => {
              if (result) {
                ar.splice(index, 1);
                saveFilters(id, filters);
              }
            });
          }).appendTo($submenu);
          $__default.default('<li><a class="dropdown-item ew-reset-filter-list" data-index="' + i + '" data-ew-action="none">' + filter[0] + '</a></li>').insertBefore($save).on("click", function (e) {
            let body = {
              cmd: "resetfilter",
              filter: JSON.stringify(filter[1])
            };
            ew.refresh(ew.fetch(ew.setLayout(ew.currentPage(), false), {
              method: "POST",
              body
            }));
          });
        });
        $__default.default('<li class="dropdown-divider"></li>').insertBefore($save);
        $delete.append($submenu);
      }
    };

    // Add event handler
    this.on = function () {
      $self.on(...arguments);
    };

    // Add event handler
    this.one = function () {
      $self.one(...arguments);
    };

    // Remove event handler
    this.off = function () {
      $self.off(...arguments);
    };

    // Trigger event
    this.trigger = function () {
      $self.trigger(...arguments);
    };

    // Init form
    this.init = function () {
      if (this._initiated) return;

      // Check form
      var form = this.getForm();
      if (!form) return;
      var $form = $__default.default(form);

      // Compile templates
      this.compileTemplates();

      // Search form
      if (/s(ea)?rch$/.test(this.id)) {
        // Search panel
        if (this.initSearchPanel && !ew.hasFormData(form)) $__default.default("#" + this.id + "_search_panel").removeClass("show");

        // Hide search operator column
        if (!$__default.default(".ew-table .ew-search-operator").text().trim()) $__default.default(".ew-table .ew-search-operator").parent("td").hide();

        // Search operators
        $form.find("select[id^=z_]").each(function () {
          var $this = $__default.default(this).trigger("change");
          if ($this.val() != "BETWEEN") $form.find("#w_" + this.id.substring(2)).trigger("change");
        });
      }

      // Multi-page
      if (this.multiPage) this.multiPage.render();

      // HTML editors
      loadjs.ready(["editor"], () => setTimeout(this.initEditors.bind(this), 0)); // Delay for custom template to apply first

      // Dynamic selection lists
      this.updateLists();

      // Init file upload
      this.initUpload();

      // Submit/Cancel
      if (this.$element.is("form")) {
        // Not Grid page
        // Detail pages
        this.$element.find(".ew-detail-pages .ew-nav a[data-bs-toggle=tab]").on("shown.bs.tab", function (e) {
          var $tab = $__default.default(e.target.getAttribute("href")),
            $panel = $tab.find(".table-responsive.ew-grid-middle-panel"),
            $container = $tab.closest(".container-fluid");
          if ($panel.width() >= $container.width()) $panel.width($container.width() + "px");else $panel.width("auto");
        });
        $form.off("submit.ew").on("submit.ew", function (e) {
          // Bind submit event
          let args = {
              form: form,
              result: self.submit(e)
            },
            evt = $__default.default.Event("aftersubmit", {
              originalEvent: e
            });
          self.trigger(evt, [args]);
          return false; // Disable normal submission
        });

        $form.find("[data-field], .ew-priv").on("change", function () {
          if (ew.CONFIRM_CANCEL) self.modified = true;
        });
        $form.find("#btn-cancel[data-href]").on("click", function () {
          // Cancel
          self.updateTextArea();
          var href = this.dataset.href;
          if (self.modified && ew.hasFormData(form)) {
            ew.prompt(ew.language.phrase("ConfirmCancel"), result => {
              if (result) {
                $form.find("#btn-action").prop("disabled", true); // Disable the save button
                window.location = href;
              }
            });
          } else {
            $form.find("#btn-action").prop("disabled", true); // Disable the save button
            window.location = href;
          }
        });
      }
      this._initiated = true;

      // Store form object as data
      this.$element.data("form", this);

      // Enable form
      if (this.enableOnInit || Array.from(form.elements).find(el => el.matches("button.disabled.enable-on-init"))) this.enableForm();

      // Trigger listeners
      this.trigger("initiated");
    };

    // Add to the global forms object
    ew.forms.add(this);
  }

  /**
   * Class Field
   */
  let Field = /*#__PURE__*/function () {
    /**
     * Constructor
     * @param {string} fldvar Field variable name
     * @param {Function[]|Function} validators Validators
     * @param {bool} invalid Initial valid status (e.g. server side)
     */
    function Field(fldvar, validators, invalid) {
      _defineProperty(this, "name", "");
      _defineProperty(this, "validators", []);
      _defineProperty(this, "_validate", true);
      this.name = fldvar;
      if (Array.isArray(validators)) {
        for (let validator of validators) this.addValidator(validator);
      } else if (typeof validators === "function") {
        this.addValidator(validators);
      }
      this.invalid = invalid;
    }

    /**
     * Add validator
     * @param {Function} validator Validator function
     */
    var _proto = Field.prototype;
    _proto.addValidator = function addValidator(validator) {
      if (typeof validator === "function") this.validators.push(validator);
    }

    /**
     * Get error
     * @returns {Object}
     */;
    /**
     * Add error
     * @param {Object} err Error
     */
    _proto.addError = function addError(err) {
      if (err) {
        var _this$_error;
        let error = (_this$_error = this._error) != null ? _this$_error : {};
        this._error = {
          ...error,
          ...err
        };
        this.invalid = true;
      }
    }

    /**
     * Clear all errors
     */;
    _proto.clearErrors = function clearErrors() {
      this._error = null;
      this.invalid = false;
    }

    /**
     * Clear all validators
     */;
    _proto.clearValidators = function clearValidators() {
      this.validators = [];
    }

    /**
     * Get error message
     * @returns {string} HTML
     */;
    /**
     * Validate field value
     * @returns {boolean}
     */
    _proto.validate = function validate() {
      let result = true;
      this.clearErrors(); // Reset error
      if (this._element && this.shouldValidate) {
        if (Array.isArray(this.validators)) {
          for (let validator of this.validators) {
            let err = validator(this._element);
            if (err !== false) {
              this.addError(err);
              result = false;
            }
          }
          this.updateFeedback();
        }
      }
      return result;
    }

    /**
     * Reset invalid property (on page load for Grid-Add/Edit)
     */;
    _proto.resetInvalid = function resetInvalid() {
      var _this$_element, _this$_element$classL, _this$_element$closes, _this$_element$closes2;
      this.clearErrors();
      if ((_this$_element = this._element) != null && (_this$_element$classL = _this$_element.classList) != null && _this$_element$classL.contains("is-invalid") && !this._error) this.addError({
        server: (_this$_element$closes = this._element.closest(ew.fieldContainerSelector)) == null ? void 0 : (_this$_element$closes2 = _this$_element$closes.querySelector(".invalid-feedback")) == null ? void 0 : _this$_element$closes2.innerHTML
      }); // Server side error
    }

    /**
     * Update the error message to feedback element
     */;
    _proto.updateFeedback = function updateFeedback() {
      let err = this.errorMessage;
      if (this._element && err) {
        var _this$_element$closes3;
        let feedback = (_this$_element$closes3 = this._element.closest(ew.fieldContainerSelector)) == null ? void 0 : _this$_element$closes3.querySelector(".invalid-feedback");
        if (feedback) feedback.innerHTML = err;
        ew.setInvalid(this._element);
      }
    }

    /**
     * Set focus
     * @param {Object} options - Focus options
     */;
    _proto.focus = function focus(options) {
      if (this._element) ew.setFocus(this._element, options);
    }

    /**
     * Check if the field can be focused
     */;
    _proto.canFocus = function canFocus() {
      var _el$style, _el$classList;
      let el = this._element;
      return el && !(el.hidden && !el.tagName == "SELECTION-LIST" || el.readonly || el.disabled || el.type == "hidden" || ((_el$style = el.style) == null ? void 0 : _el$style.display) == "none" || (_el$classList = el.classList) != null && _el$classList.contains("d-none"));
    }

    /**
     * Check if focused
     */;
    _createClass(Field, [{
      key: "error",
      get: function () {
        return this._error;
      }
    }, {
      key: "errorMessage",
      get: function () {
        if (this._error) {
          return Array.from(Object.values(this._error)).join("<br>");
        }
        return "";
      }

      /**
       * Check if the field should be validated
       */
    }, {
      key: "shouldValidate",
      get: function () {
        return !this._checkbox || this._checkbox.checked;
      }

      /**
       * Set form element
       */
    }, {
      key: "element",
      get:
      /**
       * Get form element
       * @returns {HTMLElement|HTMLElement[]}
       */
      function () {
        return this._element;
      }

      /**
       * Get field value from form element
       * @returns {string|Array}
       */,
      set: function (el) {
        var _this$_element2, _this$_element2$id;
        this._element = el;
        this._checkbox = (_this$_element2 = this._element) != null && (_this$_element2$id = _this$_element2.id) != null && _this$_element2$id.match(/^[xy]_/) ? document.getElementById(this._element.id.replace(/^[xy]_/, "u_").replace(/\[\]$/, "")) : null; // Find the checkbox for the field in Update page
      }
    }, {
      key: "value",
      get: function () {
        return this._element ? ew.getValue(this._element) : "";
      }
    }, {
      key: "focused",
      get: function () {
        return this._element && this._element == document.activeElement;
      }
    }]);
    return Field;
  }();

  /**
   * Class Form
   */
  let Form = /*#__PURE__*/function (_FormBase) {
    _inheritsLoose(Form, _FormBase);
    /**
     * Constructor
     * @param {string} id Form ID
     * @param {string} pageId Page ID
     */
    function Form(id, pageId) {
      var _this;
      _this = _FormBase.call(this, id, pageId) || this;
      _defineProperty(_assertThisInitialized(_this), "row", {});
      _defineProperty(_assertThisInitialized(_this), "fields", {});
      _defineProperty(_assertThisInitialized(_this), "validateRequired", true);
      _defineProperty(_assertThisInitialized(_this), "autoFocus", true);
      _defineProperty(_assertThisInitialized(_this), "autoFocusPreventScroll", true);
      _this.on("initiated", function () {
        let form = this.getForm();
        if (form.classList.contains("ew-wait")) {
          this.one("enabled", function () {
            this.setInvalid();
            this.tryFocus();
          });
          return;
        }
        this.setInvalid();
        this.tryFocus();
      });
      return _this;
    }

    /**
     * Add field
     * @param {string} fldvar Field variable name
     * @param {Function[]} validators Validators
     * @param {bool} invalid Invalid
     */
    var _proto = Form.prototype;
    _proto.addField = function addField(fldvar, validators, invalid) {
      if (!(fldvar in this.fields)) this.fields[fldvar] = new Field(fldvar, validators, invalid);
    }

    /**
     * Get field
     * @param {string} fldvar Field variable name
     * @returns Field
     */;
    _proto.getField = function getField(fldvar) {
      return this.fields[fldvar];
    }

    /**
     * Add fields by field definitions
     * @param {Array} fields
     */;
    _proto.addFields = function addFields(fields) {
      if (Array.isArray(fields)) {
        for (let field of fields) {
          if (Array.isArray(field)) {
            this.addField.apply(this, field);
          }
        }
      }
    }

    /**
     * Add error
     * @param {string} fldvar Field variable name
     * @param {Object} err Error
     */;
    _proto.addError = function addError(fldvar, err) {
      if (err) {
        var _this$_error;
        this._error = (_this$_error = this._error) != null ? _this$_error : {};
        this._error[fldvar] = err;
      }
    }

    /**
     * Add custom error
     * @param {string} fldvar Field variable name
     * @param {string} msg Error message
     * @param {number} rowIndex - Row index
     */;
    _proto.addCustomError = function addCustomError(fldvar, msg, rowIndex) {
      if (fldvar in this.fields) {
        let field = this.fields[fldvar],
          err = {
            custom: msg
          };
        field.addError(err);
        this.setFieldElement(fldvar, rowIndex);
        field.updateFeedback();
        this.addError(fldvar, err);
      }
      return false;
    }

    /**
     * Get error
     */;
    /**
     * Set focus to a HTML element
     * @param {HTMLElement} el - HTML element to be focused
     */
    _proto.setFocus = function setFocus(el) {
      let delay = this.makeVisible(el) ? Form.focusDelay : 0;
      if (el && el != document.activeElement && el.focus) {
        let preventScroll = !el.closest(".modal-body") && Form.autoFocusPreventScroll && this.autoFocusPreventScroll;
        setTimeout(() => {
          el.focus({
            preventScroll
          });
        }, delay); // Focus after tab transition
        this._focused = true;
      }
    }

    /**
     * Set focus to the first field with error
     */;
    _proto.focus = function focus() {
      if (!this.canFocus()) return;
      for (let [fldvar, field] of Object.entries(this.fields)) {
        var _this$_error2;
        if (field.invalid || (_this$_error2 = this._error) != null && _this$_error2[fldvar]) {
          this.getFocusable(field);
          if (field.canFocus()) {
            this.setFocus(field.element);
            break;
          }
        }
      }
    }

    /**
     * Get focuable field element
     * @param {Field} field - Field object
     */;
    _proto.getFocusable = function getFocusable(field) {
      var _field$element;
      (_field$element = field.element) != null ? _field$element : field.element = this.getFieldElements(field.name);
      if (!field.canFocus()) field.element = this.getFieldElements(field.name, 0); // Inline-Add
      if (!field.canFocus()) field.element = this.getFieldElements(field.name, 1); // Inline-Edit or Grid-Add/Edit
    }

    /**
     * Try set focus to a field
     * @param {string|undefined|true} fieldName [undefined] - Field variable name. If undefined, find the first field. If true, always try to focus.
     */;
    _proto.tryFocus = function tryFocus(fieldName) {
      if (!this.canFocus()) return;
      if (!fieldName && (!Form.autoFocus || !this.autoFocus || this._focused)) return;
      if (!fieldName && this.invalid) {
        // Has error
        this.focus();
        return;
      }
      if (["add", "edit"].includes(this.pageId)) {
        // Process detail forms
        let form = this.getForm(),
          detailpage = Array.from(form.querySelectorAll("input[name=detailpage]")).find(dp => {
            var _ew$forms$get;
            return (_ew$forms$get = ew.forms.get(dp.value)) == null ? void 0 : _ew$forms$get.invalid;
          });
        if (detailpage) {
          detailpage.focus();
          return;
        }
      }
      for (let [fldvar, field] of Object.entries(this.fields)) {
        if (typeof fieldName == "string" && fieldName !== fldvar) continue;
        field.element = null; // Reset field element first so that it will get the first element
        this.getFocusable(field);
        if (field.canFocus()) {
          this.setFocus(field.element);
          return;
        }
      }
      let input = this.element[ew.TABLE_BASIC_SEARCH]; // Quick Search input
      if (this.id.endsWith("srch") && input && input != document.activeElement) {
        // Extended Search
        input.focus({
          preventScroll: Form.autoFocusPreventScroll && this.autoFocusPreventScroll
        }); // Focus the Quick Search input
        this._focused = true;
      }
    }

    /**
     * Check if the form can be focused
     */;
    _proto.canFocus = function canFocus() {
      var _el$style, _el$classList;
      let el = this.element;
      return el && !(el.hidden || el.type == "hidden" || ((_el$style = el.style) == null ? void 0 : _el$style.display) == "none" || (_el$classList = el.classList) != null && _el$classList.contains("d-none"));
    }

    /**
     * Make the form visible
     * @param {HTMLElement} el - Focused element
     */;
    _proto.makeVisible = function makeVisible(el) {
      if (this.multiPage) {
        // Multi-page
        this.multiPage.gotoPageByElement(el);
        return true;
      } else if (this.$element.is("div")) {
        // Multiple Master/Detail
        let $pane = this.$element.closest(".tab-pane");
        if ($pane[0] && !$pane.hasClass("active")) {
          $pane.closest(".ew-nav").find("a[data-bs-toggle=tab][href='#" + $pane.attr("id") + "']").trigger("click");
          return true;
        }
      }
      return false;
    }

    /**
     * Validate all fields of the specified row
     * @param {number} rowIndex - Row index
     */;
    _proto.validateFields = function validateFields(rowIndex) {
      var _rowIndex;
      (_rowIndex = rowIndex) != null ? _rowIndex : rowIndex = this.getCurrentRowIndex();
      if (rowIndex < 2)
        // Regular pages (""), Inline-Add ("0") or first row ("1")
        this.value = null; // Reset
      this.row = {};
      this._error = null; // Reset
      let result = true;
      for (let field of Object.values(this.fields)) {
        field.element = this.getFieldElements(field.name, rowIndex);
        this.row[field.name] = field.value; // Get field value
        if (field.element && !field.validate()) {
          // Invalid field value
          this.addError(field.name, field.error);
          result = false;
        }
      }
      // Save the field values of the row
      if (!this.value) {
        this.value = {
          ...this.row
        };
      } else {
        if (!Array.isArray(this.value)) this.value = [this.value];
        let index = parseInt(rowIndex, 10) || 0;
        index = index > 1 ? index - 1 : 0;
        this.value[index] = {
          ...this.row
        };
      }
      this.focus();
      return result;
    }

    /**
     * Key count (number|NaN)
     */;
    /**
     * Validate
     * @param {Event} e - Event
     * @returns {bool}
     */
    _proto.validate = async function validate(e) {
      var _form$querySelector, _form$querySelector2, _ref, _e$currentTarget;
      if (!this.validateRequired) return true; // Ignore validation

      let form = this.getForm();
      if (((_form$querySelector = form.querySelector("#confirm")) == null ? void 0 : _form$querySelector.value) == "confirm") return true;
      if (this.pageId == "update" && !ew.updateSelected(form)) {
        ew.alert(ew.language.phrase("NoFieldSelected"));
        return false;
      }
      let addcnt = 0,
        inlineAdd = form.querySelector(".ew-inline-insert"),
        // Inline-Add
        action = ((_form$querySelector2 = form.querySelector("#action")) == null ? void 0 : _form$querySelector2.value) || ((_ref = (_e$currentTarget = e == null ? void 0 : e.currentTarget) != null ? _e$currentTarget : inlineAdd) == null ? void 0 : _ref.dataset.action),
        keycnt = inlineAdd ? 0 : this.keyCount,
        gridinsert = action == "gridinsert" || ["insert", "confirm"].includes(action) && keycnt,
        // Grid-Add or Master/Detail-Add
        startcnt = inlineAdd ? 0 : 1,
        rowcnt = inlineAdd ? 0 : keycnt || 1;
      for (let i = startcnt; i <= rowcnt; i++) {
        var _await$this$customVal, _this$customValidate;
        let rowIndex = keycnt >= 0 ? String(i) : "";
        form.dataset.rowindex = rowIndex;
        if (["list", "grid"].includes(this.pageId)) {
          if (gridinsert ? !this.emptyRow(rowIndex) : true) addcnt++;else continue;
        }

        // Validate fields
        if (!this.validateFields(rowIndex)) return false;

        // Call customValidate event
        if (!((_await$this$customVal = await ((_this$customValidate = this.customValidate) == null ? void 0 : _this$customValidate.call(this, form))) != null ? _await$this$customVal : true)) {
          this.focus();
          return false;
        }
      }
      delete form.dataset.rowindex; // Reset

      if (this.pageId == "list" && gridinsert && addcnt == 0) {
        // No row added
        ew.alert({
          html: ew.language.phrase("NoAddRecord"),
          returnFocus: false
        }); // Do not return focus
        this.tryFocus(true);
        return false;
      }

      // Process detail forms
      if (["add", "edit"].includes(this.pageId)) {
        let detailpages = form.querySelectorAll("input[name=detailpage]");
        for (let dp of detailpages) {
          var _await$frm$validate;
          let frm = ew.forms.get(dp.value);
          if (!((_await$frm$validate = await (frm == null ? void 0 : frm.validate == null ? void 0 : frm.validate(e))) != null ? _await$frm$validate : true)) return false;
        }
      }
      return true;
    }

    /**
     * Get current row index (during validation)
     * @returns {string} Row index
     */;
    _proto.getCurrentRowIndex = function getCurrentRowIndex() {
      var _this$getForm$dataset;
      return (_this$getForm$dataset = this.getForm().dataset.rowindex) != null ? _this$getForm$dataset : "";
    }

    /**
     * Get field values of the specified row
     * @param {number} rowIndex - Row index
     */;
    _proto.getValue = function getValue(rowIndex) {
      var _rowIndex2;
      (_rowIndex2 = rowIndex) != null ? _rowIndex2 : rowIndex = this.getCurrentRowIndex();
      let value = {};
      for (let field of Object.values(this.fields)) {
        var _field$element2;
        (_field$element2 = field.element) != null ? _field$element2 : field.element = this.getFieldElements(field.name, rowIndex);
        value[field.name] = field.value; // Get field value
      }

      return value;
    }

    /**
     * Get a field value of the specified row
     * @param {string} fldvar - Field variable name
     * @param {number} rowIndex - Row index
     */;
    _proto.getFieldValue = function getFieldValue(fldvar, rowIndex) {
      let field = this.getField(fldvar);
      if (field) {
        var _rowIndex3, _field$element3;
        (_rowIndex3 = rowIndex) != null ? _rowIndex3 : rowIndex = this.getCurrentRowIndex();
        (_field$element3 = field.element) != null ? _field$element3 : field.element = this.getFieldElements(field.name, rowIndex);
        return field.value;
      }
      return null;
    }

    /**
     * Get HTML elements for a field
     * @param {string} name - Field name
     * @param {number} rowIndex - Row index
     * @returns HTMLElement|HTMLElement[]|null
     */;
    _proto.getFieldElements = function getFieldElements(name, rowIndex) {
      var _rowIndex4;
      (_rowIndex4 = rowIndex) != null ? _rowIndex4 : rowIndex = this.getCurrentRowIndex();
      return this.getElements("x" + rowIndex + "_" + name) ||
      // By name with prefix
      this.getElements("x" + rowIndex + "_" + name + "[]") ||
      // By name with prefix and []
      this.getElements(name); // By name directly (e.g. email form)
    }

    /**
     * Set the element property of the field object and return the field elements
     * @param {string} fldvar - Field variable name
     * @param {number} rowIndex - Row index
     * @returns HTMLElement|HTMLElement[]|null
     */;
    _proto.setFieldElement = function setFieldElement(fldvar, rowIndex) {
      let field = this.getField(fldvar);
      if (field) field.element = this.getFieldElements(field.name, rowIndex);
      return field == null ? void 0 : field.element;
    }

    /**
     * Set invalid fields of the specified row
     * @param {number} rowIndex - Row index. If undefined, set for the whole form.
     */;
    _proto.setInvalid = function setInvalid(rowIndex) {
      let form = this.getForm(); // Get HTML form
      if (typeof rowIndex === "undefined" && this.formKeyCountName) {
        let inlineAdd = form.querySelector(".ew-inline-insert"),
          // Inline-Add
          keycnt = inlineAdd ? 0 : this.keyCount,
          startcnt = inlineAdd ? 0 : 1,
          rowcnt = inlineAdd ? 0 : keycnt || 1;
        for (let i = startcnt; i <= rowcnt; i++) {
          let rowIndex = keycnt >= 0 ? String(i) : "";
          this.setInvalid(rowIndex);
        }
      } else {
        var _rowIndex5;
        (_rowIndex5 = rowIndex) != null ? _rowIndex5 : rowIndex = "";
        for (let field of Object.values(this.fields)) {
          field.element = this.getFieldElements(field.name, rowIndex); // Always get element in case Grid-Add/Edit
          if (rowIndex) field.resetInvalid(); // For Grid-Add/Edit
          if (field.invalid) {
            this.addError(field.name, field.error);
            if (!this._focused) this.focus(); // Focus at the current row
          } else {
            continue;
          }
          ew.setInvalid(field.element);
        }
        // Process detail forms
        if (["add", "edit"].includes(this.pageId)) form.querySelectorAll("input[name=detailpage]").forEach(dp => {
          var _ew$forms$get2;
          return (_ew$forms$get2 = ew.forms.get(dp.value)) == null ? void 0 : _ew$forms$get2.setInvalid();
        });
      }
    };
    _createClass(Form, [{
      key: "error",
      get: function () {
        return this._error;
      }

      /**
       * Check if invalid
       */
    }, {
      key: "invalid",
      get: function () {
        return this._error || Object.values(this.fields).some(field => field.invalid);
      }
    }, {
      key: "keyCount",
      get: function () {
        var _this$getForm$querySe;
        let keycnt = this.formKeyCountName ? (_this$getForm$querySe = this.getForm().querySelector("#" + this.formKeyCountName)) == null ? void 0 : _this$getForm$querySe.value : undefined; // Get key_count
        return parseInt(keycnt, 10);
      }
    }]);
    return Form;
  }(FormBase);
  _defineProperty(Form, "autoFocus", true);
  _defineProperty(Form, "autoFocusPreventScroll", true);
  _defineProperty(Form, "focusDelay", 200);

  /**
   * Class FormBuilder
   */
  let FormBuilder = /*#__PURE__*/function () {
    function FormBuilder() {
      _defineProperty(this, "lists", {});
      _defineProperty(this, "fields", []);
      _defineProperty(this, "validateRequired", false);
      _defineProperty(this, "multiPage", false);
      _defineProperty(this, "submitWithFetch", false);
      _defineProperty(this, "initSearchPanel", false);
      _defineProperty(this, "enableOnInit", false);
    }
    var _proto = FormBuilder.prototype;
    // Set ID
    _proto.setId = function setId(value) {
      this.id = value;
      return this;
    }

    // Set page ID
    ;
    _proto.setPageId = function setPageId(value) {
      this.pageId = value;
      return this;
    }

    // Set lists
    ;
    _proto.setLists = function setLists(value) {
      if (value && typeof value == "object" && !Array.isArray(value)) this.lists = value;
      return this;
    }

    // Set lists for query builder (Dynamic Selection Lists and AutoFill not supported since there can be multiple inputs for the same field)
    ;
    _proto.setQueryBuilderLists = function setQueryBuilderLists(value) {
      if (value && typeof value == "object" && !Array.isArray(value)) {
        for (let [id, list] of Object.entries(value)) value[id] = {
          ...list,
          ...{
            autoFillTargetFields: [],
            childFields: [],
            filterFieldVars: [],
            filterFields: [],
            parentFields: []
          }
        };
        this.lists = value;
      }
      return this;
    }

    // Add list
    ;
    _proto.addList = function addList(name, value) {
      this.lists[name] = value;
      return this;
    }

    // Set fields
    ;
    _proto.setFields = function setFields(value) {
      if (Array.isArray(value)) this.fields = value;
      return this;
    }

    // Add field
    ;
    _proto.addField = function addField(fldvar, validators, invalid) {
      if (!this.fields.find(field => field[0] == fldvar)) this.fields.push([fldvar, validators, invalid]);
      return this;
    }

    // Add fields
    ;
    _proto.addFields = function addFields(value) {
      if (Array.isArray(value)) this.fields = [...this.fields, ...value];
      return this;
    }

    // Set validate
    ;
    _proto.setValidate = function setValidate(value) {
      this.validate = value;
      return this;
    }

    // Set custom validate
    ;
    _proto.setCustomValidate = function setCustomValidate(value) {
      this.customValidate = value;
      return this;
    }

    // Set validate required
    ;
    _proto.setValidateRequired = function setValidateRequired(value) {
      this.validateRequired = value;
      return this;
    }

    // Set submit
    ;
    _proto.setSubmit = function setSubmit(value) {
      this.submit = value;
      return this;
    }

    // Set form KeyCount name
    ;
    _proto.setFormKeyCountName = function setFormKeyCountName(value) {
      this.formKeyCountName = value;
      return this;
    }

    // Set empty row
    ;
    _proto.setEmptyRow = function setEmptyRow(value) {
      this.emptyRow = value;
      return this;
    }

    // Set multi page
    ;
    _proto.setMultiPage = function setMultiPage(value) {
      this.multiPage = value;
      return this;
    }

    // Set submit with Fetch API
    ;
    _proto.setSubmitWithFetch = function setSubmitWithFetch(value) {
      this.submitWithFetch = value;
      return this;
    }

    // Set filter list
    ;
    _proto.setFilterList = function setFilterList(value) {
      this.filterList = value;
      return this;
    }

    // Set initSearchPanel
    ;
    _proto.setInitSearchPanel = function setInitSearchPanel(value) {
      this.initSearchPanel = value;
      return this;
    }

    // Set enableOnInit
    ;
    _proto.setEnableOnInit = function setEnableOnInit(value) {
      this.enableOnInit = value;
      return this;
    }

    // Build
    ;
    _proto.build = function build() {
      let form = new ew.Form(this.id, this.pageId);
      if (Array.isArray(this.fields)) form.addFields(this.fields);
      if (typeof this.validate == 'function') form.validate = this.validate;
      if (typeof this.customValidate == 'function') form.customValidate = this.customValidate;
      if (typeof this.emptyRow == 'function') form.emptyRow = this.emptyRow;
      if (typeof this.submit == 'function') form.submit = this.submit;
      if (this.formKeyCountName) form.formKeyCountName = this.formKeyCountName;
      if (this.multiPage) form.multiPage = new ew.MultiPage(this.id);
      if (this.enableOnInit) form.enableOnInit = true;
      if (this.submitWithFetch) {
        form.submitWithFetch = true;
        form.on("aftersubmit", (e, args) => ew.refresh(args == null ? void 0 : args.result, args == null ? void 0 : args.context));
      }
      if (this.filterList) form.filterList = this.filterList;
      if (this.lists) form.lists = this.lists;
      form.validateRequired = !!this.validateRequired;
      form.initSearchPanel = !!this.initSearchPanel;
      form.lists = this.lists;
      return form;
    };
    return FormBuilder;
  }();

  // AjaxLookup class
  let AjaxLookup = /*#__PURE__*/function () {
    /**
     * Constructor
     * @param {Object} settings Settings
     * @param {string} settings.id - Input element ID
     * @param {string|Form} settings.form - Form of the input element
     * @param {Number} settings.limit - Options per page
     * @param {Object} settings.data - Data submitted by Ajax
     * @param {string} settings.action - Ajax action: "autosuggest" or "modal"
     */
    function AjaxLookup(settings) {
      var _this$list$parentFiel, _this$list$parentFiel2;
      _defineProperty(this, "_isAutoSuggest", null);
      this.elementId = settings.id; // Id
      this.form = settings.form; // Form
      if ($__default.default.isString(this.form))
        // Form is string => Form id
        this.form = ew.forms.get(this.form);
      this.element = this.form.getElement(this.elementId); // Actual HTML element
      this.formElement = this.form.getElement(); // HTML form or DIV
      this.list = this.form.getList(this.element);
      let m = this.elementId.match(/^[xy](\d*|\$rowindex\$)_/),
        rowindex = m ? m[1] : "";
      this.parentFields = (_this$list$parentFiel = (_this$list$parentFiel2 = this.list.parentFields) == null ? void 0 : _this$list$parentFiel2.slice() // Clone
      .map(pf => pf.split(" ").length == 1 ? pf.replace(/^x_/, "x" + rowindex + "_") : pf)) != null ? _this$list$parentFiel : []; // Parent field in the same table, add row index
      this.limit = settings.limit;
      this.debounce = settings.debounce;
      this.data = settings.data;
      this.recordCount = 0;
      this.action = settings.action || "autosuggest";
    }

    /**
     * Is AutoSuggest
     */
    var _proto = AjaxLookup.prototype;
    /**
     * Format display value
     * @param {Array} opt Option
     */
    _proto.formatResult = function formatResult(opt) {
      this.form.compileTemplates();
      return this.list.template && !this.isAutoSuggest ? this.list.template.render(opt, ew.jsRenderHelpers) : ew.displayValue(opt, this.element) || opt[0];
    }

    /**
     * Generate request
     */;
    _proto.generateRequest = function generateRequest() {
      var _data$ajax;
      var data = Object.assign({}, this.data, {
        name: this.element.name,
        page: this.list.page,
        field: this.list.field,
        language: ew.LANGUAGE_ID
      }, ew.getUserParams("#p_" + this.elementId, this.formElement));
      (_data$ajax = data.ajax) != null ? _data$ajax : data.ajax = this.action;
      if (this.parentFields.length > 0) this.parentFields.forEach((pf, i) => data["v" + (i + 1)] = ew.getOptionValue(pf, this.formElement));
      return data;
    }

    /**
     * Get URL
     */;
    _proto.getUrl = function getUrl(query, start) {
      let params = new URLSearchParams({
        q: query || "",
        n: this.limit,
        rnd: ew.random(),
        start: $__default.default.isNumber(start) ? start : -1
      });
      return ew.getApiUrl(ew.API_LOOKUP_ACTION, params.toString());
    }

    /**
     * Prepare URL and data for sending request
     * @param {string} query Search term
     * @param {Number} start Start page
     */;
    _proto.prepare = function prepare(query, start) {
      return {
        url: this.getUrl(query, start),
        type: "POST",
        dataType: "json",
        data: this.generateRequest()
      };
    }

    /**
     * Transform options (virtual)
     * @param {Object[]} data Data from server
     */;
    _proto.transform = function transform(data) {
      let results = [];
      if ((data == null ? void 0 : data.result) == "OK") {
        this.recordCount = data.totalRecordCount;
        results = data.records;
      }
      return results;
    }

    /**
     * Convert options to Select2 data format
     * @param {Object[]} options Options
     * @returns {Object[]}
     */;
    _proto.toSelect2Data = function toSelect2Data(options) {
      if (Array.isArray(options)) {
        if (options.every(item => item.hasOwnProperty("gf"))) {
          // Has group by field
          let groups = [...new Set(options.map(item => item.gf))];
          return groups.map(group => {
            return {
              text: group,
              children: options.filter(item => item.gf == group).map(item => {
                return {
                  id: item.lf,
                  text: this.formatResult({
                    lf: item.lf,
                    df: item.df,
                    df2: item.df2,
                    df3: item.df3,
                    df4: item.df4
                  })
                };
              })
            };
          });
        } else {
          return options.map(item => {
            return {
              id: item.lf,
              text: this.formatResult({
                lf: item.lf,
                df: item.df,
                df2: item.df2,
                df3: item.df3,
                df4: item.df4
              })
            };
          });
        }
      }
      return options;
    };
    _createClass(AjaxLookup, [{
      key: "isAutoSuggest",
      get: function () {
        var _this$_isAutoSuggest;
        (_this$_isAutoSuggest = this._isAutoSuggest) != null ? _this$_isAutoSuggest : this._isAutoSuggest = ew.isAutoSuggest(this.element);
        return this._isAutoSuggest;
      }

      /**
       * Lookup options
       */
    }, {
      key: "options",
      get: function () {
        return this.list.lookupOptions;
      }
    }]);
    return AjaxLookup;
  }();

  // AutoSuggest class
  let AutoSuggest = /*#__PURE__*/function (_AjaxLookup) {
    _inheritsLoose(AutoSuggest, _AjaxLookup);
    function AutoSuggest(settings) {
      var _this;
      _this = _AjaxLookup.call(this, settings) || this;
      _this.input = _this.form.getElement("sv_" + _this.elementId); // User input
      if (!_this.input || _this.elementId.includes("$rowindex$")) return _assertThisInitialized(_this);
      let self = _assertThisInitialized(_this),
        $input = $__default.default(_this.input),
        $element = $__default.default(_this.element);

      // Properties
      _this.minWidth = settings.minWidth;
      _this.maxHeight = settings.maxHeight;
      _this.highlight = settings.highlight;
      _this.hint = settings.hint;
      _this.minLength = settings.minLength;
      _this.templates = Object.assign({}, settings.templates);
      _this.classNames = Object.assign({}, settings.classNames);
      _this.delay = settings.delay; // For loading more results
      _this.debounce = settings.debounce;
      _this.display = settings.display || "text";
      _this.forceSelection = settings.forceSelect;
      _this.lineHeight = settings.lineHeight;
      _this.paddingY = settings.paddingY;
      _this.lookupAllDisplayFields = settings.lookupAllDisplayFields;
      _this.$input = $input;
      _this.$element = $element;

      // Save instance
      $element.data("autosuggest", _assertThisInitialized(_this));

      // Save initial option
      if ($input.val() && $element.val()) _this.element.add($element.val(), $input.val(), true);

      // Add events
      $input.on("typeahead:select", (e, d) => {
        self.setValue(d[self.display]);
      }).on("change", () => {
        let ta = $input.data("tt-typeahead");
        if (ta != null && ta.isOpen() && !ta.menu.empty()) {
          let $item = ta.menu.getActiveSelectable();
          if ($item) {
            // A suggestion is highlighted
            let i = $item.index(),
              val = self.element.options[i].text;
            $input.typeahead("val", val);
          }
        }
        self.setValue();
      }).on("blur", () => {
        // "change" fires before blur
        let ta = $input.data("tt-typeahead");
        if (ta != null && ta.isOpen()) ta.menu.close();
      }).on("focus", () => {
        $input.attr("placeholder", $input.data("placeholder")).removeClass("is-invalid");
        $element.removeClass("is-invalid");
      });

      // Get suggestions
      let async = !_this.options.length,
        loadingMore = false,
        timer;

      // Option template ("suggestion" template)
      let tpl = self.list.template || self.templates.suggestion;
      if (tpl && $__default.default.isString(tpl)) tpl = $__default.default.templates(tpl);
      if (tpl) self.templates.suggestion = tpl.render.bind(tpl);
      if (async && !self.templates.footer) self.templates.footer = '<div class="tt-footer dropdown-item"><div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">' + ew.language.phrase("LoadingMore") + '</span></div></div>'; // "footer" template

      let source = (query, syncResults, asyncResults) => {
        if (async) {
          if (timer) timer.cancel();
          timer = $__default.default.later(_this.debounce, null, () => {
            _this.recordCount = 0; // Reset
            $__default.default.ajax(_this.prepare(query)).done(data => asyncResults(_this.transform(data)));
          });
        } else {
          let records = _this.getSyncResults(query);
          syncResults(_this.transform({
            result: "OK",
            totalRecordCount: records.length,
            records
          }));
        }
      };

      // Create Typeahead
      $__default.default(function () {
        // Typeahead options and dataset
        let options = {
          highlight: self.highlight,
          minLength: self.minLength,
          hint: self.hint,
          classNames: self.classNames
        };
        let dataset = {
          name: self.form.id + "-" + self.elementId,
          source,
          async,
          templates: self.templates,
          display: self.display,
          limit: async ? self.limit : Infinity
        };
        let args = [options, dataset];
        // Trigger "typeahead" event
        $element.trigger("typeahead", [args]);
        // Create Typeahead
        self.typeahead = $input.typeahead.apply($input, args).off("blur.tt").data("tt-typeahead");
        let menu = self.typeahead.menu,
          $menu = menu.$node,
          $dataset = $menu.find(".tt-dataset"),
          suggestionHeight = () => $menu.find(".tt-suggestion").outerHeight(false);
        if (self.minWidth) $menu.css("min-width", self.minWidth);
        $input.on("typeahead:rendered", (e, suggestions) => {
          let rendered = suggestions.length,
            count = self.count;
          if (count >= self.limit) {
            let h = suggestionHeight();
            if (h) $dataset.css("max-height", h * self.limit);
          }
          if (rendered > 0) $dataset.scrollTop(suggestionHeight() * (count - rendered)); // Scroll to the first suggestion
          if (async) $menu.find(".tt-footer").toggle(self.recordCount > count);
        });
        if (async) {
          let loadingMoreTimer;
          $dataset.on("scroll", () => {
            var _loadingMoreTimer;
            (_loadingMoreTimer = loadingMoreTimer) == null ? void 0 : _loadingMoreTimer.cancel();
            loadingMoreTimer = $__default.default.later(self.delay, null, () => {
              let $footer = $menu.find(".tt-footer");
              if (!$footer.is(":hidden") && !loadingMore) {
                let currentOffset = $dataset.offset().top + $dataset.outerHeight(false),
                  loadingMoreOffset = $footer.offset().top + $footer.outerHeight(false);
                if (currentOffset + 20 > loadingMoreOffset) {
                  // $footer shows more than 20px
                  loadingMore = true;
                  self.getMore().always(() => loadingMore = false);
                } else {
                  var _loadingMoreTimer2;
                  (_loadingMoreTimer2 = loadingMoreTimer) == null ? void 0 : _loadingMoreTimer2.cancel();
                }
              }
            });
          });
        }
      });
      return _this;
    }

    // Set the selected item to the actual field
    var _proto = AutoSuggest.prototype;
    _proto.setValue = function setValue(v) {
      v || (v = this.$input.val());
      let index = this.element.options.findIndex(option => option.text == v);
      if (index < 0) {
        // Not found in results
        if (this.forceSelection && v) {
          // Force selection and query not empty => error
          this.$input.typeahead("val", "").addClass("is-invalid");
          this.$element.next(".invalid-feedback").html(ew.language.phrase("ValueNotExist"));
          this.$element.addClass("is-invalid").val("").trigger("change");
          return;
        }
      } else {
        // Found in results
        this.element.options[index].selected = true;
        if (!/s(ea)?rch$/.test(this.formElement.id) || this.forceSelection)
          // Force selection or not search form
          v = this.element.options[index].value; // Replace the display value by Link Field value
      }

      if (v !== this.$element.attr("value")) this.$element.attr("value", v).trigger("change"); // Set value to the actual field
    }

    // Transform suggestion
    ;
    _proto.transform = function transform(data) {
      let results = _AjaxLookup.prototype.transform.call(this, data).map(item => Object.assign({}, item, {
        text: _AjaxLookup.prototype.formatResult.call(this, item)
      }));
      this.element.options = results.map(item => new ew.SelectionListOption(item.text, item.lf || item[0]));
      return results;
    }

    // Get current suggestion count
    ;
    // Get suggestions from lookup cache
    _proto.getSyncResults = function getSyncResults(query) {
      if (this.options.length) {
        let results = this.options.filter(item => {
          if (this.lookupAllDisplayFields) {
            let v = [item.df, item.df2, item.df3, item.df4].map(df => String(df).toLowerCase()).join(" ");
            return query.toLowerCase().split(" ").filter(q => q !== "").every(q => v.includes(q));
          } else {
            return String(item.df).toLowerCase().startsWith(query);
          }
        });
        this.recordCount = results.length;
        return results;
      }
      return [];
    }

    // Get more suggestions by Ajax
    ;
    _proto.getMore = function getMore() {
      let menu = this.typeahead.menu,
        start = this.count,
        settings = this.prepare(menu.query, start);
      return $__default.default.ajax(settings).done(data => menu.datasets[0]._append(menu.query, this.transform(data)));
    };
    _createClass(AutoSuggest, [{
      key: "count",
      get: function () {
        return this.typeahead.menu.$node.find(".tt-suggestion.tt-selectable").length || 0;
      }
    }]);
    return AutoSuggest;
  }(AjaxLookup);

  /**
   * Class Forms
   */
  let Forms = /*#__PURE__*/function () {
    function Forms() {
      _defineProperty(this, "_forms", {});
    }
    var _proto = Forms.prototype;
    /**
     * Get form by element or id
     * @param {HTMLElement|string} el Element or id
     */
    _proto.get = function get(el) {
      var _ew$getForm;
      if (!el) return null;
      let id = $__default.default.isString(el) ? el : (_ew$getForm = ew.getForm(el)) == null ? void 0 : _ew$getForm.id;
      return this._forms[id];
    }

    /**
     * Add form
     * @param {Form} f Form
     */;
    _proto.add = function add(f) {
      if (this._forms[f.id] && this._forms[f.id] !== f) delete this._forms[f.id];
      this._forms[f.id] = f;
    }

    /**
     * Get all ids
     * @returns {string[]}
     */;
    _proto.ids = function ids() {
      return Object.keys(this._forms);
    };
    return Forms;
  }();

  let lang = ew.language.phrase("select2");
  var Select2Language = {
    errorLoading: function () {
      return lang.errorLoading;
    },
    inputTooLong: function (args) {
      var overChars = args.input.length - args.maximum;
      return lang.inputTooLong.replace("%s", overChars);
    },
    inputTooShort: function (args) {
      var remainingChars = args.minimum - args.input.length;
      return lang.inputTooShort.replace("%s", remainingChars);
    },
    loadingMore: function () {
      return '<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">' + lang.loadingMore + '</span></div>';
    },
    maximumSelected: function (args) {
      return lang.maximumSelected.replace("%s", args.maximum);
    },
    noResults: function () {
      return lang.noResults;
    },
    searching: function () {
      return '<div class="spinner-border spinner-border-sm text-primary" role="status"><span class="visually-hidden">' + lang.searching + '</span></div>';
    },
    removeAllItems: function () {
      return lang.removeAllItems;
    },
    removeItem: function () {
      return lang.removeItem;
    },
    search: function () {
      return lang.search;
    }
  };

  let _defined$3 = $__default.default.fn.select2.amd.require._defined,
    Utils$3 = _defined$3['select2/utils'];

  /**
   * Select2 decorator for Results
   */
  let Select2ResultsDecorator = /*#__PURE__*/function () {
    function Select2ResultsDecorator() {}
    var _proto = Select2ResultsDecorator.prototype;
    _proto.render = function render(decorated) {
      var $results = $__default.default('<div class="select2-results__options ' + this.options.get('containerClass') + '" role="listbox"></div>'); //***

      if (this.options.get('multiple')) {
        $results.attr('aria-multiselectable', 'true');
      }
      this.$results = $results;
      return $results;
    };
    _proto.displayMessage = function displayMessage(decorated, params) {
      var escapeMarkup = this.options.get('escapeMarkup');
      this.clear();
      this.hideLoading();
      var $message = $__default.default('<div role="alert" aria-live="assertive"' + ' class="select2-results__option"></div>'); //***

      if (params.message.includes("<") && params.message.includes(">")) {
        // HTML //***
        $message.append(params.message);
      } else {
        var message = this.options.get('translations').get(params.message);
        $message.append(escapeMarkup(message(params.args)));
      }
      $message[0].className += ' select2-results__message';
      this.$results.append($message);
    };
    _proto.append = function append(decorated, data) {
      this.hideLoading();
      if (data.results == null || data.results.length === 0) {
        if (this.$results.children().length === 0) {
          if (this.$element.data("updating") && data.pagination.more) {
            this.trigger('results:message', {
              message: '<div class="spinner-border spinner-border-sm text-primary ew-select-spinner" role="status"><span class="visually-hidden">' + ew.language.phrase('Loading') + '</span></div> ' + ew.language.phrase('Loading')
            });
            this.$element.one("updated", () => this.$element.select2("close").select2("open"));
          } else {
            this.trigger('results:message', {
              message: 'noResults'
            });
          }
        }
        return;
      }
      data.results = this.sort(data.results);

      //***
      var cols = this.options.get('columns'),
        len = data.results.length,
        $row = this.$results.find("." + this.options.get('rowClass')).last();
      for (var d = 0; d < data.results.length; d++) {
        var item = data.results[d];
        var $option = this.option(item);
        if (!$row.length || $row.children().length == cols) {
          // Add new row
          $row = $__default.default('<div class="' + this.options.get('rowClass') + '"></div>');
          this.$results.append($row);
        }
        $row.append($option);
        if (d == len - 1) {
          // Last
          var cnt = cols - $row.children().length;
          for (var i = 0; i < cnt; i++) $row.append('<div class="' + this.options.get('cellClass') + '"></div>');
        }
      }
    };
    _proto.option = function option(decorated, data) {
      // var option = document.createElement('li');
      var option = document.createElement('div'); //***
      option.classList.add('select2-results__option');
      option.classList.add('select2-results__option--selectable');
      this.options.get('cellClass').split(" ").forEach(c => option.classList.add(c)); //***

      var attrs = {
        'role': 'option',
        'aria-selected': 'false'
      };
      var matches = window.Element.prototype.matches || window.Element.prototype.msMatchesSelector || window.Element.prototype.webkitMatchesSelector;
      if (data.element != null && matches.call(data.element, ':disabled') || data.element == null && data.disabled) {
        attrs['aria-disabled'] = 'true';
        option.classList.remove('select2-results__option--selectable');
        option.classList.add('select2-results__option--disabled');
      }
      if (data.id == null) {
        option.classList.remove('select2-results__option--selectable');
      }
      if (data._resultId != null) {
        option.id = data._resultId;
      }
      if (data.title) {
        option.title = data.title;
      }

      // if (data.children) { //***
      //   attrs.role = 'group';
      //   attrs['aria-label'] = data.text;
      //   option.classList.remove('select2-results__option--selectable');
      //   option.classList.add('select2-results__option--group');
      // }

      for (var attr in attrs) {
        var val = attrs[attr];
        option.setAttribute(attr, val);
      }

      // if (data.children) { //***
      //   var $option = $(option);

      //   var label = document.createElement('strong');
      //   label.className = 'select2-results__group';

      //   this.template(data, label);

      //   var $children = [];

      //   for (var c = 0; c < data.children.length; c++) {
      //     var child = data.children[c];

      //     var $child = this.option(child);

      //     $children.push($child);
      //   }

      //   var $childrenContainer = $('<ul></ul>', {
      //     'class': 'select2-results__options select2-results__options--nested',
      //     'role': 'none'
      //   });

      //   $childrenContainer.append($children);

      //   $option.append(label);
      //   $option.append($childrenContainer);
      // } else {
      this.template(data, option);
      // }

      Utils$3.StoreData(option, 'data', data);
      return option;
    };
    return Select2ResultsDecorator;
  }();

  let _defined$2 = $__default.default.fn.select2.amd.require._defined,
    Utils$2 = _defined$2['select2/utils'];

  /**
   * Results for modal lookup
   */
  let ModalResults = /*#__PURE__*/function () {
    function ModalResults() {}
    var _proto = ModalResults.prototype;
    _proto.bind = function bind(decorated, container, $container) {
      var _container$listeners$;
      var self = this;
      decorated.call(this, container, $container);

      // Remove handlers
      (_container$listeners$ = container.listeners['results:select']) == null ? void 0 : _container$listeners$.pop();
      this.$results.off('mouseup');
      container.on('results:select', function (evt) {
        var $highlighted = self.getHighlightedResults();
        if ($highlighted.length === 0) {
          return;
        }
        var data = Utils$2.GetData($highlighted[0], 'data');
        if ($highlighted.hasClass('select2-results__option--selected')) {
          self.trigger('unselect', {
            originalEvent: evt,
            data: data
          });
        } else {
          self.trigger('select', {
            originalEvent: evt,
            data: data
          });
        }
      });
      this.$results.on('mousedown', '.select2-results__option--selectable', function (evt) {
        this._mousedown = true;
      });
      this.$results.on('mouseup', '.select2-results__option--selectable', function (evt) {
        if (!this._mousedown) return;
        var $this = $__default.default(this);
        var data = Utils$2.GetData(this, 'data');
        if ($this.hasClass('select2-results__option--selected')) {
          self.trigger('unselect', {
            originalEvent: evt,
            data: data
          });
          return;
        }
        self.trigger('select', {
          originalEvent: evt,
          data: data
        });
      });
    };
    return ModalResults;
  }();

  /**
   * Search box for modal lookup
   */
  let ModalSearch = /*#__PURE__*/function () {
    function ModalSearch() {}
    var _proto = ModalSearch.prototype;
    _proto.bind = function bind(decorated, container, $container) {
      var _container$listeners$;
      var self = this;
      decorated.call(this, container, $container);
      (_container$listeners$ = container.listeners['close']) == null ? void 0 : _container$listeners$.pop(); // Remove handler from Search

      container.on('close', function () {
        self.$search.attr('tabindex', -1).removeAttr('aria-controls').removeAttr('aria-activedescendant');
      });
    };
    return ModalSearch;
  }();

  /**
   * Dropdown search decorator
   */
  let KEYS = $.fn.select2.amd.require._defined['select2/keys'];
  KEYS.PRINT_SCREEN = 44;
  let DropdownSearchDecorator = /*#__PURE__*/function () {
    function DropdownSearchDecorator() {}
    var _proto = DropdownSearchDecorator.prototype;
    _proto.handleSearch = function handleSearch(decorated, evt) {
      var key = evt.which;

      // Ignore events from modifier keys
      if ([KEYS.TAB, KEYS.SHIFT, KEYS.CTRL, KEYS.ALT, KEYS.PRINT_SCREEN].includes(key)) {
        return;
      }
      if (!this._keyUpPrevented) {
        var input = this.$search.val();
        this.trigger('query', {
          term: input
        });
      }
      this._keyUpPrevented = false;
    };
    return DropdownSearchDecorator;
  }();

  let AttachBody$2 = $__default.default.fn.select2.amd.require._defined['select2/dropdown/attachBody'];

  /**
   * Select2 AttachBody with popper
   */
  let Select2AttachBody = /*#__PURE__*/function (_AttachBody) {
    _inheritsLoose(Select2AttachBody, _AttachBody);
    // Constructor
    function Select2AttachBody(decorated, $element, options) {
      return _AttachBody.call(this, decorated, $element, options) || this;
    }

    // Override _attachPositioningHandler
    var _proto = Select2AttachBody.prototype;
    _proto._attachPositioningHandler = function _attachPositioningHandler(decorated, container) {
      var self = this;
      var events = ['scroll.select2.' + container.id, 'resize.select2.' + container.id, 'orientationchange.select2.' + container.id];
      var handler = () => {
        self._positionDropdown();
        self._resizeDropdown();
      };
      $__default.default(window).on(events.join(' '), handler);
      container.$element.closest('.modal').on('scroll.select2.' + container.id, handler);
    };
    // Override _detachPositioningHandler
    _proto._detachPositioningHandler = function _detachPositioningHandler(decorated, container) {
      var events = ['scroll.select2.' + container.id, 'resize.select2.' + container.id, 'orientationchange.select2.' + container.id];
      $__default.default(window).off(events.join(' '));
      container.$element.closest('.modal').off('scroll.select2.' + container.id);
    };
    // Override _showDropdown
    _proto._showDropdown = function _showDropdown(decorated) {
      var _this$_popper;
      this.$dropdownContainer.appendTo(this.$dropdownParent);
      (_this$_popper = this._popper) != null ? _this$_popper : this._popper = Popper.createPopper(this.$container[0], this.$dropdownContainer[0], {
        placement: ew.IS_RTL ? 'bottom-end' : 'bottom-start',
        modifiers: [{
          name: 'flip',
          enabled: true
        }, {
          name: 'preventOverflow',
          enabled: true
        }]
      });
      this._positionDropdown();
      this._resizeDropdown();
    };
    // Override _positionDropdown()
    _proto._positionDropdown = function _positionDropdown() {
      var _this$_popper2;
      (_this$_popper2 = this._popper) == null ? void 0 : _this$_popper2.update();
    }

    // Override destroy()
    ;
    _proto.destroy = function destroy(decorated) {
      var _this$_popper3;
      _AttachBody.prototype.destroy.call(this, decorated);
      (_this$_popper3 = this._popper) == null ? void 0 : _this$_popper3.destroy();
      this._popper = null;
    };
    return Select2AttachBody;
  }(AttachBody$2);

  /**
   * Select2 AttachBody decorator for modal lookup
   */
  let ModalAttachBody = /*#__PURE__*/function () {
    function ModalAttachBody(decorated, $element, options) {
      _defineProperty(this, "$modal", null);
      options.set('dropdownParent', $__default.default(document.body));
      decorated.call(this, $element, options);
    }
    var _proto = ModalAttachBody.prototype;
    _proto.bind = function bind(decorated, container, $container) {
      let self = this;
      decorated.call(this, container, $container);
      container.on('open', function () {
        self._showDropdown();

        // Must bind after the results handlers to ensure correct sizing
        self._bindContainerResultHandlers(container);
      });
      this.$dropdownContainer.on('mousedown', function (evt) {
        evt.stopPropagation();
      });
    };
    _proto.position = function position(decorated, $dropdown, $container) {
      // Clone all of the container classes
      $dropdown.attr('class', $container.attr('class'));
      $dropdown.removeClass('select2');
      this.$container = $container;
    };
    _proto.render = function render(decorated) {
      let $container = $__default.default('<span></span>');
      let $dropdown = decorated.call(this);
      $container.append($dropdown);
      this.$dropdownContainer = $container;
      return $container;
    };
    _proto._bindContainerResultHandlers = function _bindContainerResultHandlers(decorated, container) {
      // These should only be bound once
      if (this._containerResultsHandlersBound) {
        return;
      }
      let self = this;
      container.$modal = this.$modal;
      container.on('results:all', function () {
        var _self$$search$;
        self._updateDropdown();
        (_self$$search$ = self.$search[0]) == null ? void 0 : _self$$search$.focus();
      });
      container.on('results:append', function () {
        self._updateDropdown();
      });
      container.on('results:message', function () {
        self._updateDropdown();
      });
      container.on('select', function (e) {
        var _e$originalEvent;
        let target = (_e$originalEvent = e.originalEvent) == null ? void 0 : _e$originalEvent.currentTarget;
        target == null ? void 0 : target.classList.add('select2-results__option--selected');
        self._updateDropdown();
      });
      container.on('unselect', function (e) {
        var _e$originalEvent2;
        let target = (_e$originalEvent2 = e.originalEvent) == null ? void 0 : _e$originalEvent2.currentTarget;
        target == null ? void 0 : target.classList.remove('select2-results__option--selected');
        self._updateDropdown();
      });
      this._containerResultsHandlersBound = true;
    };
    _proto._updateDropdown = function _updateDropdown() {
      if (!this.$modal.find(this.$dropdownContainer)[0]) this.$modal.find('.modal-body').children().detach().end().append(this.$dropdownContainer);
    };
    _proto._showDropdown = function _showDropdown(decorated) {
      var _this$$modal;
      let self = this,
        oldValue = this.$element.val();
      this.$dropdownContainer.appendTo(this.options.get('dropdownParent'));
      (_this$$modal = this.$modal) != null ? _this$$modal : this.$modal = $__default.default('#ew-modal-lookup-dialog');
      this._updateDropdown();
      this.$modal.find('.modal-title').empty().append(ew.language.phrase('LookupTitle').replace('%s', this.$element.data('caption')));
      this.$modal.find('.modal-footer button[data-value]').off().on('click', function () {
        if (!$__default.default(this).data('value'))
          // Cancel
          self.$element.val(oldValue).trigger('change');
      });
      this.$modal.modal('show').on('hidden.bs.modal', function (event) {
        self.$container.removeClass('select2-container--open');
      }).draggable(this.options.get('draggableOptions'));
    };
    return ModalAttachBody;
  }();

  let AttachBody$1 = $__default.default.fn.select2.amd.require._defined['select2/dropdown/attachBody'];

  /**
   * Select2 AttachBody for table header filter
   */
  let FilterAttachBody = /*#__PURE__*/function (_AttachBody) {
    _inheritsLoose(FilterAttachBody, _AttachBody);
    // Constructor
    function FilterAttachBody(decorated, $element, options) {
      return _AttachBody.call(this, decorated, $element, options) || this;
    }

    // Override bind()
    var _proto = FilterAttachBody.prototype;
    _proto.bind = function bind(decorated, container, $container) {
      var self = this;
      decorated.call(this, container, $container);
      container.on('open', function () {
        self._showDropdown();

        // Must bind after the results handlers to ensure correct sizing
        self._bindContainerResultHandlers(container);
      });
      container.on('close', function () {
        self._hideDropdown();
      });
      this.$dropdownContainer.on('mousedown', function (evt) {
        evt.stopPropagation();
      });
    }

    // Override _positionDropdown()
    ;
    _proto._positionDropdown = function _positionDropdown() {
      var _this$_popper;
      (_this$_popper = this._popper) == null ? void 0 : _this$_popper.update();
    }

    // Override _resizeDropdown()
    ;
    _proto._resizeDropdown = function _resizeDropdown() {
      var css = {
        width: this.$container.outerWidth(false) + 'px'
      };
      if (this.options.get('dropdownAutoWidth')) {
        css.minWidth = css.width;
        css.position = 'relative';
        css.width = 'auto';
      }
      this.$dropdown.css(css);
    }

    // Override _showDropdown()
    ;
    _proto._showDropdown = function _showDropdown(decorated) {
      var _dropdownButton$close, _this$_popper2;
      this.$dropdownContainer.appendTo(this.$dropdownParent);

      // Footer
      let self = this,
        oldValue = this.$element.val(),
        $footer = $__default.default('#ew-filter-dropdown-footer').contents().clone();
      $footer.find('.ew-filter-btn[data-value]').off('click.ew').on('click.ew', function (e) {
        let value = $__default.default(this).data('value');
        if (value) {
          // OK
          $__default.default(self.$element[0].form).triggerHandler('submit');
        } else {
          // Cancel
          self.$element.val(oldValue).trigger('change');
        }
        self.$element.select2('close');
      });
      $footer.find('.ew-filter-clear').off('click.ew').on('click.ew', e => self.$element.data('select2').selection._handleClear(e));
      let $filterDropdown = this.$dropdownContainer.find('.ew-filter-dropdown');
      if (!$filterDropdown.find('.ew-filter-btn')[0]) $filterDropdown.append($footer);

      // Popper
      var dropdownButton = document.querySelector('.ew-filter-dropdown-btn[data-table=' + this.$element.data('table') + '][data-field=' + this.$element.data('field') + ']'),
        reference = (_dropdownButton$close = dropdownButton.closest(".ew-table-header-cell")) != null ? _dropdownButton$close : dropdownButton.closest(".ew-table-header-btn");
      (_this$_popper2 = this._popper) != null ? _this$_popper2 : this._popper = Popper.createPopper(reference, this.$dropdownContainer[0], {
        placement: ew.IS_RTL ? 'bottom-end' : 'bottom-start',
        modifiers: [{
          name: 'flip',
          enabled: true
        }, {
          name: 'preventOverflow',
          enabled: true
        }]
      });
      this._positionDropdown();
      this._resizeDropdown();
    }

    // Override position()
    ;
    _proto.position = function position(decorated, $dropdown, $container) {
      // Clone all of the container classes
      $dropdown.attr('class', $container.attr('class'));
      $dropdown[0].classList.remove('select2');
      $dropdown[0].classList.add('select2-container--open');
      this.$container = $container;
    };
    // Override destroy()
    _proto.destroy = function destroy(decorated) {
      var _this$_popper3;
      _AttachBody.prototype.destroy.call(this, decorated);
      (_this$_popper3 = this._popper) == null ? void 0 : _this$_popper3.destroy();
      this._popper = null;
    };
    return FilterAttachBody;
  }(AttachBody$1);

  let AttachBody = $__default.default.fn.select2.amd.require._defined['select2/dropdown/attachBody'];

  /**
   * Select2 AttachBody for dropdown
   */
  let DropdownAttachBody = /*#__PURE__*/function (_AttachBody) {
    _inheritsLoose(DropdownAttachBody, _AttachBody);
    // Constructor
    function DropdownAttachBody(decorated, $element, options) {
      return _AttachBody.call(this, decorated, $element, options) || this;
    }

    // Override bind()
    var _proto = DropdownAttachBody.prototype;
    _proto.bind = function bind(decorated, container, $container) {
      var self = this;
      decorated.call(this, container, $container);
      container.on('open', function () {
        self._showDropdown();

        // Must bind after the results handlers to ensure correct sizing
        self._bindContainerResultHandlers(container);
      });
      container.on('close', function () {
        self._hideDropdown();
      });
      this.$dropdownContainer.on('mousedown', function (evt) {
        evt.stopPropagation();
      });
    }

    // Override _showDropdown()
    ;
    _proto._showDropdown = function _showDropdown(decorated) {
      var _this$_popper;
      this.$dropdownContainer.appendTo(this.$dropdownParent);
      (_this$_popper = this._popper) != null ? _this$_popper : this._popper = Popper.createPopper(this.$element.parent().find('.select2-container')[0], this.$dropdownContainer[0], {
        placement: ew.IS_RTL ? 'bottom-end' : 'bottom-start',
        modifiers: [{
          name: 'flip',
          enabled: true
        }, {
          name: 'preventOverflow',
          enabled: true
        }]
      });
      this._positionDropdown();
      this._resizeDropdown();
    }

    // Override _positionDropdown()
    ;
    _proto._positionDropdown = function _positionDropdown() {
      var _this$_popper2;
      (_this$_popper2 = this._popper) == null ? void 0 : _this$_popper2.update();
    }

    // Override position()
    ;
    _proto.position = function position(decorated, $dropdown, $container) {
      // Clone all of the container classes
      $dropdown.attr('class', $container.attr('class'));
      $dropdown[0].classList.remove('select2');
      $dropdown[0].classList.add('select2-container--open');
      this.$container = $container;
    };
    // Override destroy()
    _proto.destroy = function destroy(decorated) {
      var _this$_popper3;
      _AttachBody.prototype.destroy.call(this, decorated);
      (_this$_popper3 = this._popper) == null ? void 0 : _this$_popper3.destroy();
      this._popper = null;
    };
    return DropdownAttachBody;
  }(AttachBody);

  let _defined$1 = $__default.default.fn.select2.amd.require._defined,
    AllowClear$1 = _defined$1['select2/selection/allowClear'],
    Utils$1 = _defined$1['select2/utils'];

  /**
   * Select2 AttachBody for table header filter
   */
  let FilterAllowClear = /*#__PURE__*/function (_AllowClear) {
    _inheritsLoose(FilterAllowClear, _AllowClear);
    function FilterAllowClear() {
      return _AllowClear.apply(this, arguments) || this;
    }
    var _proto = FilterAllowClear.prototype;
    // Override _handleClear
    _proto._handleClear = function _handleClear(_, evt) {
      // Ignore the event if it is disabled
      if (this.isDisabled()) {
        return;
      }
      var $clear = this.$selection.find('.select2-selection__clear');

      // Ignore the event if nothing has been selected
      if ($clear.length === 0) {
        return;
      }
      evt.stopPropagation();
      var data = Utils$1.GetData($clear[0], 'data');
      var previousVal = this.$element.val();
      this.$element.val(this.placeholder.id);
      var unselectData = {
        data: data
      };
      this.trigger('clear', unselectData);
      if (unselectData.prevented) {
        this.$element.val(previousVal);
        return;
      }
      for (var d = 0; d < data.length; d++) {
        unselectData = {
          data: data[d]
        };

        // Trigger the `unselect` event, so people can prevent it from being
        // cleared.
        this.trigger('unselect', unselectData);

        // If the event was prevented, don't clear it out.
        if (unselectData.prevented) {
          this.$element.val(previousVal);
          return;
        }
      }
      this.$element.trigger('input').trigger('change');
    };
    return FilterAllowClear;
  }(AllowClear$1);

  /**
   * Select2 decorator for MultipleSelection
   */
  let Select2MultipleSelectionDecorator = /*#__PURE__*/function () {
    function Select2MultipleSelectionDecorator() {}
    var _proto = Select2MultipleSelectionDecorator.prototype;
    _proto.bind = function bind(decorated, container, $container) {
      decorated.call(this, container, $container);
      this.$selection.on('click', '.select2-selection__choice__remove', function (evt) {
        evt.stopPropagation();
      });
    };
    return Select2MultipleSelectionDecorator;
  }();

  /**
   * Select2 decorator for SelectAdapter
   */
  let Select2DataAdapterDecorator = /*#__PURE__*/function () {
    function Select2DataAdapterDecorator() {}
    var _proto = Select2DataAdapterDecorator.prototype;
    _proto.option = function option(decorated, data) {
      var _data$element;
      var text = data.text,
        html = (_data$element = data.element) == null ? void 0 : _data$element.innerHTML,
        $option = decorated.call(this, data);

      // Check HTML
      if (text.startsWith('<') && text.endsWith('>')) $option.html(text);else if (html && html != text) $option.html(html);
      return $option;
    };
    return Select2DataAdapterDecorator;
  }();

  // Defined classes from select2
  let _defined = $.fn.select2.amd.require._defined,
    ResultsList = _defined['select2/results'],
    SingleSelection = _defined['select2/selection/single'],
    MultipleSelection = _defined['select2/selection/multiple'],
    Placeholder = _defined['select2/selection/placeholder'],
    AllowClear = _defined['select2/selection/allowClear'],
    SelectionSearch = _defined['select2/selection/search'],
    EventRelay = _defined['select2/selection/eventRelay'],
    Utils = _defined['select2/utils'],
    Translation = _defined['select2/translation'],
    SelectData = _defined['select2/data/select'],
    ArrayData = _defined['select2/data/array'],
    AjaxData = _defined['select2/data/ajax'],
    Tags = _defined['select2/data/tags'],
    Tokenizer = _defined['select2/data/tokenizer'],
    MinimumInputLength = _defined['select2/data/minimumInputLength'],
    MaximumInputLength = _defined['select2/data/maximumInputLength'],
    MaximumSelectionLength = _defined['select2/data/maximumSelectionLength'],
    Dropdown = _defined['select2/dropdown'],
    DropdownSearch = _defined['select2/dropdown/search'],
    HidePlaceholder = _defined['select2/dropdown/hidePlaceholder'],
    InfiniteScroll = _defined['select2/dropdown/infiniteScroll'],
    // AttachBody = _defined['select2/dropdown/attachBody'],
    MinimumResultsForSearch = _defined['select2/dropdown/minimumResultsForSearch'],
    SelectOnClose = _defined['select2/dropdown/selectOnClose'],
    CloseOnSelect = _defined['select2/dropdown/closeOnSelect'],
    DropdownCSS = _defined['select2/dropdown/dropdownCss'],
    TagsSearchHighlight = _defined['select2/dropdown/tagsSearchHighlight'],
    Defaults = _defined['select2/defaults'];

  // Override select2 Defaults
  Defaults.apply = function (options) {
    options = $.extend(true, {}, this.defaults, options);
    if (options.dataAdapter == null) {
      if (options.ajax != null) {
        options.dataAdapter = AjaxData;
      } else if (options.data != null) {
        options.dataAdapter = ArrayData;
      } else {
        options.dataAdapter = SelectData;
      }
      options.dataAdapter = Utils.Decorate(
      // Override
      options.dataAdapter, Select2DataAdapterDecorator);
      if (options.minimumInputLength > 0) {
        options.dataAdapter = Utils.Decorate(options.dataAdapter, MinimumInputLength);
      }
      if (options.maximumInputLength > 0) {
        options.dataAdapter = Utils.Decorate(options.dataAdapter, MaximumInputLength);
      }
      if (options.maximumSelectionLength > 0) {
        options.dataAdapter = Utils.Decorate(options.dataAdapter, MaximumSelectionLength);
      }
      if (options.tags) {
        options.dataAdapter = Utils.Decorate(options.dataAdapter, Tags);
      }
      if (options.tokenSeparators != null || options.tokenizer != null) {
        options.dataAdapter = Utils.Decorate(options.dataAdapter, Tokenizer);
      }
    }
    if (options.resultsAdapter == null) {
      options.resultsAdapter = ResultsList;

      // Override
      if (options.columns > 0 && options.customOption) {
        options.resultsAdapter = Utils.Decorate(options.resultsAdapter, Select2ResultsDecorator);
        if (options.iconClass && options.multiple && options.templateResult == ew.selectOptions.templateResult) {
          options._templateResult = options.templateResult;
          options.templateResult = result => result.loading ? result.text : '<div class="form-check-input ew-dropdown-check-input"></div><label class="' + options.iconClass + ' ew-dropdown-check-label">' + options._templateResult(result) + '</label>';
        }
      } else if (options.modal || options.filter) {
        options.resultsAdapter = Utils.Decorate(options.resultsAdapter, ModalResults);
      }
      if (options.ajax != null) {
        options.resultsAdapter = Utils.Decorate(options.resultsAdapter, InfiniteScroll);
      }
      if (options.placeholder != null) {
        options.resultsAdapter = Utils.Decorate(options.resultsAdapter, HidePlaceholder);
      }
      if (options.selectOnClose) {
        options.resultsAdapter = Utils.Decorate(options.resultsAdapter, SelectOnClose);
      }
      if (options.tags) {
        options.resultsAdapter = Utils.Decorate(options.resultsAdapter, TagsSearchHighlight);
      }
    }
    if (options.dropdownAdapter == null) {
      if (options.modal || options.filter) {
        options.dropdownAdapter = Utils.Decorate(Dropdown, DropdownSearch);
        options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, DropdownSearchDecorator);
        options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, ModalSearch);
      } else {
        options.dropdownAdapter = Dropdown;
        if (!options.multiple) {
          options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, DropdownSearch);
          options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, DropdownSearchDecorator);
        }
      }
      if (options.minimumResultsForSearch !== 0) {
        options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, MinimumResultsForSearch);
      }
      if (options.closeOnSelect) {
        options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, CloseOnSelect);
      }
      if (options.dropdownCssClass != null) {
        options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, DropdownCSS);
      }
      options.dropdownAdapter = Utils.Decorate(options.dropdownAdapter, options.modal ? ModalAttachBody : options.filter ? FilterAttachBody : options.dropdown ? DropdownAttachBody : Select2AttachBody // Override
      );
    }

    if (options.selectionAdapter == null) {
      if (options.multiple) {
        options.selectionAdapter = MultipleSelection;
        options.selectionAdapter = Utils.Decorate(
        // Override
        options.selectionAdapter, Select2MultipleSelectionDecorator);
      } else {
        options.selectionAdapter = SingleSelection;
      }

      // Add the placeholder mixin if a placeholder was specified
      if (options.placeholder != null) {
        options.selectionAdapter = Utils.Decorate(options.selectionAdapter, Placeholder);
      }
      if (options.allowClear) {
        options.selectionAdapter = Utils.Decorate(options.selectionAdapter, options.filter ? FilterAllowClear : AllowClear);
      }
      if (options.multiple) {
        options.selectionAdapter = Utils.Decorate(options.selectionAdapter, SelectionSearch);
      }
      options.selectionAdapter = Utils.Decorate(options.selectionAdapter, EventRelay);
    }
    options.translations = new Translation(Select2Language);
    options.dir = ew.IS_RTL ? "rtl" : "ltr";
    return options;
  };

  let currentUrl = new URL(window.location);
  let forms = new Forms();
  let $document$1 = $__default.default(document),
    $body = $__default.default("body");
  let fieldContainerSelector = ".row, [id^=el_], [class^=el_]";

  // Set focus
  Pace.on("done", () => {
    var _forms$get;
    let form = document.querySelector(".modal.show form.ew-form") || document.querySelector("form.ew-form");
    (_forms$get = forms.get(form == null ? void 0 : form.id)) == null ? void 0 : _forms$get.tryFocus();
  });

  // Create select2
  function createSelect(options) {
    if (options.selectId.includes("$rowindex$")) return;
    if ($__default.default.isObject(options.data)) {
      let lookup = new ew.AjaxLookup(options.data);
      options.data = lookup.toSelect2Data(lookup.options);
    }
    if ($__default.default.isObject(options.ajax)) {
      let limit = options.ajax.limit,
        lookup = new ew.AjaxLookup({
          ...options.ajax,
          ...{
            action: "modal"
          }
        });
      options.ajax = {
        url: params => {
          let start = params.page ? (params.page - 1) * limit : -1;
          return lookup.getUrl(params.term, start);
        },
        type: "POST",
        dataType: "json",
        data: lookup.generateRequest.bind(lookup),
        delay: options.debounce,
        processResults: function (data) {
          var _data$records$length, _data$records;
          let self = this;
          return {
            results: lookup.toSelect2Data(lookup.transform(data)),
            pagination: {
              more: self.container.$results.find(".select2-results__option:not(.select2-results__option--load-more)").length + ((_data$records$length = (_data$records = data.records) == null ? void 0 : _data$records.length) != null ? _data$records$length : 0) < lookup.recordCount
            }
          };
        },
        transport: (params, success, failure) => $__default.default.ajax(params).then(data => {
          let error = getError(data);
          !error || _alert(error);
          success(data);
        }).fail(failure)
      };
    }
    let args = {
      name: options.name,
      options
    };
    $document$1.trigger("select2", [args]);
    let $select = $__default.default("select[data-select2-id='" + options.selectId + "']").select2(args.options);
    $select.on("select2:open", function () {
      var _$$data$$dropdown$fin;
      (_$$data$$dropdown$fin = $__default.default(this).data("select2").$dropdown.find(".select2-search__field")[0]) == null ? void 0 : _$$data$$dropdown$fin.focus();
    });
    if ($__default.default.isObject(options.ajax)) {
      $select.on("select2:opening", function () {
        $__default.default(this).data("select2").$results.find(".select2-results__option:not(.loading-results)").remove();
      });
    }
    if (options.minimumResultsForSearch === Infinity) {
      $select.on("select2:opening select2:closing", function () {
        $__default.default(this).data("select2").$dropdown.find(".select2-search--dropdown").addClass("select2-search--hide");
      });
    }
  }

  // Create modal lookup
  function createModalLookup(options) {
    if (options.selectId.includes("$rowindex$")) return;
    if ($__default.default.isObject(options.data)) {
      let lookup = new ew.AjaxLookup(options.data);
      options.data = lookup.options.map(item => {
        return {
          id: item.lf,
          text: lookup.formatResult({
            lf: item.lf,
            df: item.df,
            df2: item.df2,
            df3: item.df3,
            df4: item.df4
          })
        };
      });
    }
    if ($__default.default.isObject(options.ajax)) {
      let limit = options.ajax.limit,
        lookup = new ew.AjaxLookup({
          ...options.ajax,
          ...{
            action: "modal"
          }
        });
      options.ajax = {
        url: params => {
          let start = params.page ? (params.page - 1) * limit : -1;
          return lookup.getUrl(params.term, start);
        },
        type: "POST",
        dataType: "json",
        data: lookup.generateRequest.bind(lookup),
        delay: options.debounce,
        processResults: function (data) {
          var _data$records$length2, _data$records2;
          let self = this;
          return {
            results: lookup.transform(data).map(item => {
              return {
                id: item.lf,
                text: lookup.formatResult({
                  lf: item.lf,
                  df: item.df,
                  df2: item.df2,
                  df3: item.df3,
                  df4: item.df4
                })
              };
            }),
            pagination: {
              more: self.container.$results.find(".select2-results__option:not(.select2-results__option--load-more)").length + ((_data$records$length2 = (_data$records2 = data.records) == null ? void 0 : _data$records2.length) != null ? _data$records$length2 : 0) < lookup.recordCount
            }
          };
        },
        transport: (params, success, failure) => $__default.default.ajax(params).then(data => {
          let error = getError(data);
          !error || _alert(error);
          success(data);
        }).fail(failure)
      };
    }
    let $select = $__default.default("select[data-select2-id='" + options.selectId + "']").select2(options);
    $select.on("select2:open", function () {
      var _$$data$$dropdown$fin2;
      (_$$data$$dropdown$fin2 = $__default.default(this).data("select2").$dropdown.find(".select2-search__field").addClass("form-control")[0]) == null ? void 0 : _$$data$$dropdown$fin2.focus();
    });
    if ($__default.default.isObject(options.ajax)) {
      $select.on("select2:opening", function () {
        $__default.default(this).data("select2").$results.find(".select2-results__option:not(.loading-results)").remove();
      });
    }
  }

  // Create table header filter
  function createFilter(options) {
    if (options.selectId.includes("$rowindex$")) return;
    if ($__default.default.isObject(options.data)) {
      let lookup = new ew.AjaxLookup(options.data);
      options.data = lookup.options.map(item => {
        return {
          id: item.lf,
          text: lookup.formatResult({
            lf: item.lf,
            df: item.df,
            df2: item.df2,
            df3: item.df3,
            df4: item.df4
          })
        };
      });
    }
    if ($__default.default.isObject(options.ajax)) {
      let limit = options.ajax.limit,
        lookup = new ew.AjaxLookup({
          ...options.ajax,
          ...{
            action: "modal"
          }
        });
      options.ajax = {
        url: params => {
          let start = params.page ? (params.page - 1) * limit : -1;
          return lookup.getUrl(params.term, start);
        },
        type: "POST",
        dataType: "json",
        data: lookup.generateRequest.bind(lookup),
        delay: options.debounce,
        processResults: function (data) {
          var _data$records$length3, _data$records3;
          let self = this;
          return {
            results: lookup.transform(data).map(item => {
              return {
                id: item.lf,
                text: lookup.formatResult({
                  lf: item.lf,
                  df: item.df,
                  df2: item.df2,
                  df3: item.df3,
                  df4: item.df4
                })
              };
            }),
            pagination: {
              more: self.container.$results.find(".select2-results__option:not(.select2-results__option--load-more)").length + ((_data$records$length3 = (_data$records3 = data.records) == null ? void 0 : _data$records3.length) != null ? _data$records$length3 : 0) < lookup.recordCount
            }
          };
        },
        transport: (params, success, failure) => $__default.default.ajax(params).then(data => {
          let error = getError(data);
          !error || _alert(error);
          success(data);
        }).fail(failure)
      };
    }
    let $select = $__default.default("select[data-select2-id='" + options.selectId + "']").select2(options);
    $select.on("select2:open", function () {
      var _$$data$$dropdown$fin3;
      (_$$data$$dropdown$fin3 = $__default.default(this).data("select2").$dropdown.find(".select2-search__field").addClass("form-control")[0]) == null ? void 0 : _$$data$$dropdown$fin3.focus({
        preventScroll: options.preventScroll
      }); // Do not scroll on focus by default
    });

    if ($__default.default.isObject(options.ajax)) {
      $select.on("select2:opening", function () {
        $__default.default(this).data("select2").$results.find(".select2-results__option:not(.loading-results)").remove();
      });
    }
  }

  // Init icon tooltip
  function initIcons(e) {
    var _e$target;
    let el = (_e$target = e == null ? void 0 : e.target) != null ? _e$target : document,
      tooltipOptions = {
        ...ew.tooltipOptions,
        container: "body",
        trigger: ew.IS_MOBILE ? "manual" : "hover"
      };
    $__default.default(el).find(".ew-icon").closest(".btn, .ew-home, .ew-row-link:not(.dropdown-item)").each(function () {
      let $this = $__default.default(this);
      if ($this.hasClass("dropdown-toggle")) {
        let $p = $this.closest(".btn-group");
        if ($p.children(".btn").length == 1) {
          $p.tooltip({
            ...tooltipOptions,
            title: this.dataset.title || this.title
          }).on("mouseleave", e => {
            var _bootstrap$Tooltip$ge;
            return (_bootstrap$Tooltip$ge = bootstrap.Tooltip.getInstance(e.currentTarget)) == null ? void 0 : _bootstrap$Tooltip$ge.hide();
          });
          $this.next(".dropdown-menu").on("mouseover", e => e.stopPropagation());
        }
      } else {
        $this.tooltip(tooltipOptions);
      }
    });
  }

  // Init password options
  function initPasswordOptions(e) {
    var _e$target2;
    var el = (_e$target2 = e == null ? void 0 : e.target) != null ? _e$target2 : document;
    if ($__default.default.fn.pStrength && typeof ew.MIN_PASSWORD_STRENGTH != "undefined") {
      $__default.default(el).find(".ew-password-strength").each(function () {
        var $this = $__default.default(this);
        if (!$this.data("pStrength")) $this.pStrength({
          "changeBackground": false,
          "backgrounds": [],
          "passwordValidFrom": ew.MIN_PASSWORD_STRENGTH,
          "onPasswordStrengthChanged": function (strength, percentage) {
            var $pst = $__default.default("[id='" + this.attr("data-password-strength") + "']"),
              // Do not use #
              $pb = $pst.find(".progress-bar");
            if (this.val() && !ew.isMaskedPassword(this)) {
              var pct = percentage + "%",
                min = ew.MIN_PASSWORD_STRENGTH,
                valid = percentage >= min;
              if (percentage < min * 0.25) {
                $pb.addClass("bg-danger").removeClass("bg-warning bg-info bg-success");
              } else if (percentage < min * 0.5) {
                $pb.addClass("bg-warning").removeClass("bg-danger bg-info bg-success");
              } else if (percentage < min * 0.75) {
                $pb.addClass("bg-primary").removeClass("bg-danger bg-warning bg-success");
              } else {
                $pb.addClass("bg-success").removeClass("bg-danger bg-warning bg-info");
              }
              $pb.css("width", pct);
              if (percentage > min * 0.5) pct = ew.language.phrase("PasswordStrength").replace("%p", pct);
              $pb.html(pct);
              $pst.removeClass("d-none");
              this.data("validated", valid);
              if (valid) setValid(this[0]);
            } else {
              $pst.addClass("d-none");
              this.data("validated", null);
            }
            $pst.width(this.outerWidth());
          }
        });
      });
    }
    if ($__default.default.fn.pGenerator) {
      $__default.default(el).find(".ew-password-generator").each(function () {
        var $this = $__default.default(this);
        if (!$this.data("pGenerator")) $this.pGenerator({
          "passwordLength": ew.GENERATE_PASSWORD_LENGTH,
          "uppercase": ew.GENERATE_PASSWORD_UPPERCASE,
          "lowercase": ew.GENERATE_PASSWORD_LOWERCASE,
          "numbers": ew.GENERATE_PASSWORD_NUMBER,
          "specialChars": ew.GENERATE_PASSWORD_SPECIALCHARS,
          "onPasswordGenerated": function (pwd) {
            $__default.default("#" + this.attr("data-password-confirm")).val(pwd);
            $__default.default("#" + this.attr("data-password-field")).val(pwd).trigger("change").trigger("focus").triggerHandler("click"); // Trigger click to remove "is-invalid" class (Do not use $this.data)
          }
        });
      });
    }
  }

  /**
   * Get API action URL
   * @param {string|string[]} action - Route as string or array, e.g. "foo", ["foo", "1"]
   * @param {string|string[]|Object|URLSearchParams|FormData} query - Search params, e.g. "foo=1&bar=2", [["foo", "1"], ["bar", "2"]], {"foo": "1", "bar": "2"}
   */
  function getApiUrl(action, query) {
    var url = ew.PATH_BASE + ew.API_URL,
      params = query instanceof URLSearchParams ? query : new URLSearchParams(query),
      qs = params.toString();
    if ($__default.default.isString(action)) {
      // Route as string
      url += action ? action : "";
    } else if (Array.isArray(action)) {
      // Route as array
      var route = action.map(function (v) {
        return encodeURIComponent(v);
      }).join("/");
      url += route ? route : "";
    }
    return url + (qs ? "?" + qs : "");
  }

  /**
   * Parse URL
   * Note: .search is read only, always use .searchParams to change parameters
   * @param {string} url - URL
   * @returns {Object}
   */
  function parseUrl(url) {
    var _pathname, _hash;
    let pathname, search, hash;
    [pathname, hash] = url.split("#");
    [pathname, search] = pathname.split("?");
    if (!search && pathname.includes("=")) [pathname, search] = ["", pathname];
    let obj = {
      url: url,
      pathname: (_pathname = pathname) != null ? _pathname : "",
      hash: (_hash = hash) != null ? _hash : "",
      searchParams: new URLSearchParams(search)
    };
    Object.defineProperty(obj, "search", {
      // Read only
      value: search,
      writable: false,
      enumerable: true
    });
    return obj;
  }

  /**
   * Create URLSearchParams
   * @param {*} params - search parameters
   * @returns {URLSearchParams}
   */
  function createSearchParams(params) {
    try {
      return new URLSearchParams(params);
    } catch (e) {
      console.log(e);
      return new URLSearchParams();
    }
  }

  /**
   * Get URL search parameters
   * @param {string} url - URL
   * @returns {URLSearchParams}
   */
  function getSearchParams(url) {
    return parseUrl(url).searchParams;
  }

  /**
   * Get a search parameter from URL
   * @param {string} url - URL
   * @param {string} name - Name
   * @returns {string} Value
   */
  function getSearchParam(url, name) {
    return getSearchParams(url).get(name);
  }

  /**
   * Set URL search parameters
   * @param {string} url - URL
   * @param {*} args - search parameters to be merged
   * @returns {string} URL
   */
  function setSearchParams(url) {
    let obj = parseUrl(url);
    for (var _len = arguments.length, args = new Array(_len > 1 ? _len - 1 : 0), _key = 1; _key < _len; _key++) {
      args[_key - 1] = arguments[_key];
    }
    obj.searchParams = mergeSearchParams(obj.searchParams, ...args);
    return buildUrl(obj);
  }

  /**
   * Merge URL search parameters
   * @param {*} args - search parameters to be merged
   * @returns {URLSearchParams}
   */
  function mergeSearchParams() {
    let searchParams = new URLSearchParams();
    for (var _len2 = arguments.length, args = new Array(_len2), _key2 = 0; _key2 < _len2; _key2++) {
      args[_key2] = arguments[_key2];
    }
    for (const params of args) {
      if ($__default.default.isObject(params) || $__default.default.isString(params) && params) {
        // Note: Array is also object
        let newParams = params instanceof URLSearchParams ? params : createSearchParams(params);
        newParams.forEach((value, key) => searchParams.has(key) ? key.endsWith("[]") ? searchParams.append(key, value) : searchParams.set(key, value) : searchParams.set(key, value));
      }
    }
    return searchParams;
  }

  /**
   * Set URL search parameter
   * @param {string} url - URL
   * @param {string} name - Name
   * @param {string} value - Value
   * @returns {string} URL
   */
  function setSearchParam(url, name, value) {
    return setSearchParams(url, {
      [name]: value
    });
  }

  /**
   * Delete search parameters from URL
   * @param {string} url - URL
   * @param {string[]} names - Names
   * @returns {string} URL
   */
  function deleteSearchParam(url) {
    let obj = parseUrl(url);
    for (var _len3 = arguments.length, names = new Array(_len3 > 1 ? _len3 - 1 : 0), _key3 = 1; _key3 < _len3; _key3++) {
      names[_key3 - 1] = arguments[_key3];
    }
    names.forEach(name => obj.searchParams.delete(name));
    return buildUrl(obj);
  }

  /**
   * Build URL
   * @param {Object} obj Object { pathname, searchParams, hash }
   * @returns {string} URL
   */
  function buildUrl(obj) {
    let search = obj.searchParams.toString();
    return obj.pathname + (obj.pathname && search ? "?" : "") + search + (obj.hash ? "#" + obj.hash : "");
  }

  /**
   * Set layout parameter to URL
   * Disable layout by adding "layout=false"
   * @param {string} url - URL
   * @param {string} value - Layout
   * @returns {string} URL
   */
  function setLayout(url, value) {
    return url && url != "#" ? setSearchParams(url, {
      [ew.PAGE_LAYOUT]: String(value)
    }) : url;
  }

  // Sanitize URL
  function sanitizeUrl(url) {
    let obj = parseUrl(url),
      searchParams = obj.searchParams;
    searchParams.forEach((value, key) => {
      value = decodeURIComponent(value);
      if (["<>", "<=", ">=", ">", "<"].includes(value)) searchParams.set(key, value);else searchParams.set(key, ew.sanitize(value));
    });
    return buildUrl(obj);
  }

  // Set session timer
  function setSessionTimer() {
    var timeoutTime,
      timer,
      keepAliveTimer,
      counter,
      useKeepAlive = ew.SESSION_KEEP_ALIVE_INTERVAL > 0 || ew.IS_LOGGEDIN && ew.IS_AUTOLOGIN;
    // Keep alive
    var keepAlive = () => {
      $__default.default.get(getApiUrl(ew.API_SESSION_ACTION), {
        "rnd": random()
      }, token => {
        if (token && $__default.default.isObject(token)) {
          // PHP
          ew.TOKEN_NAME = token[ew.TOKEN_NAME_KEY];
          ew.ANTIFORGERY_TOKEN = token[ew.ANTIFORGERY_TOKEN_KEY];
          if (token["JWT"]) ew.API_JWT_TOKEN = token["JWT"];
        }
      });
    };
    // Reset timer
    var resetTimer = () => {
      counter = ew.SESSION_TIMEOUT_COUNTDOWN;
      timeoutTime = ew.SESSION_TIMEOUT - ew.SESSION_TIMEOUT_COUNTDOWN;
      if (timeoutTime < 0) {
        // Timeout now
        timeoutTime = 0;
        counter = ew.SESSION_TIMEOUT;
      }
      if (timer) timer.cancel(); // Clear timer
    };
    // Timeout
    var timeout = () => {
      if (keepAliveTimer) keepAliveTimer.cancel(); // Stop keep alive
      if (counter > 0) {
        let timerInterval;
        let message = '<p class="text-danger">' + ew.language.phrase("SessionWillExpire") + '</p>';
        if (message.includes("%m") && message.includes("%s")) {
          message = message.replace("%m", '<span class="ew-session-counter-minute">' + Math.floor(counter / 60) + '</span>');
          message = message.replace("%s", '<span class="ew-session-counter-second">' + counter % 60 + '</span>');
        } else if (message.includes("%s")) {
          message = message.replace("%s", '<span class="ew-session-counter-second">' + counter + '</span>');
        }
        Swal.fire({
          ...ew.sweetAlertSettings,
          html: message,
          showConfirmButton: true,
          confirmButtonText: ew.language.phrase("OKBtn"),
          timer: counter * 1000,
          timerProgressBar: true,
          allowOutsideClick: false,
          allowEscapeKey: false,
          willOpen: () => {
            timerInterval = setInterval(() => {
              let content = Swal.getHtmlContainer(),
                min = content.querySelector(".ew-session-counter-minute"),
                sec = content.querySelector(".ew-session-counter-second"),
                timeleft = Math.round(Swal.getTimerLeft() / 1000);
              if (min && sec) {
                min.textContent = Math.floor(timeleft / 60);
                sec.textContent = timeleft % 60;
              } else if (sec) {
                sec.textContent = timeleft;
              }
            }, 1000);
          },
          willClose: () => {
            clearInterval(timerInterval);
          }
        }).then(result => {
          if (result.value) {
            // OK button pressed
            keepAlive();
            if (!useKeepAlive && ew.SESSION_TIMEOUT > 0) setTimer();
          } else if (result.dismiss === Swal.DismissReason.timer) {
            // Timeout
            resetTimer();
            window.location = sanitizeUrl(ew.TIMEOUT_URL + "?expired=1");
          }
        });
      }
    };
    // Set timer
    var setTimer = () => {
      resetTimer(); // Reset timer first
      timer = $__default.default.later(timeoutTime * 1000, null, timeout);
    };
    if (useKeepAlive) {
      // Keep alive
      var keepAliveInterval = ew.SESSION_KEEP_ALIVE_INTERVAL > 0 ? ew.SESSION_KEEP_ALIVE_INTERVAL : ew.SESSION_TIMEOUT - ew.SESSION_TIMEOUT_COUNTDOWN;
      if (keepAliveInterval <= 0) keepAliveInterval = 60;
      keepAliveTimer = $__default.default.later(keepAliveInterval * 1000, null, keepAlive, null, true); // Periodic
    } else {
      if (ew.SESSION_TIMEOUT > 0)
        // Set session timeout
        setTimer();
    }
  }

  // Init export links
  function initExportLinks(e) {
    var _e$target3;
    let $el = $__default.default((_e$target3 = e == null ? void 0 : e.target) != null ? _e$target3 : document);
    $el.find("a.ew-export-link[href]:not(.ew-email):not(.ew-print):not(.ew-xml)").on("click", function (e) {
      let href = this.href;
      if (href && href != "#") fileDownload(href, getchartParams());
      e.preventDefault();
    });
    $el.find("a.ew-export-link[href].ew-print, a.ew-export-link[href].ew-xml").each(function () {
      let url = new URL(this.href);
      url.searchParams.set(ew.TOKEN_NAME_KEY, ew.TOKEN_NAME);
      url.searchParams.set(ew.ANTIFORGERY_TOKEN_KEY, ew.ANTIFORGERY_TOKEN);
      this.href = url.pathname + url.search; // Add token name and antiforgery token // PHP
    });
  }

  // Init multi-select checkboxes
  function initMultiSelectCheckboxes(e) {
    var _e$target4;
    var el = (_e$target4 = e == null ? void 0 : e.target) != null ? _e$target4 : document,
      $el = $__default.default(el),
      $cbs = $el.find("input[type=checkbox].ew-multi-select");
    var _update = function (id) {
      var $els = $cbs.filter("[name^='" + id + "_']"),
        cnt = $els.length,
        len = $els.filter(":checked").length;
      $__default.default("input[type=checkbox]#" + id).prop("checked", len == cnt).prop("indeterminate", len != cnt && len != 0);
    };
    $cbs.on("click", e => _update(e.target.name.split("_")[0]));
    $el.find("input[type=checkbox].ew-priv:not(.ew-multi-select)").each((i, el) => _update(el.id)); // Init
  }

  // Get error from response
  function getError(result) {
    var _result$error, _result$error2;
    return result && (((_result$error = result.error) == null ? void 0 : _result$error.description) || ((_result$error2 = result.error) == null ? void 0 : _result$error2.message) || $__default.default.isString(result.error) && result.error || result.failureMessage ||
    // Check this first
    result.success === false && result.message || null);
  }

  // Download file
  function fileDownload(href, data) {
    let isHtml = href.includes("export=html") || href.includes(getApiUrl([ew.API_EXPORT_ACTION, "html"])),
      swal = window.parent.Swal; // Use window.parent.Swal in case in iframe
    data = mergeSearchParams(data, [[ew.TOKEN_NAME_KEY, ew.TOKEN_NAME], [ew.ANTIFORGERY_TOKEN_KEY, ew.ANTIFORGERY_TOKEN]]);
    return swal.fire({
      ...ew.sweetAlertSettings,
      showConfirmButton: false,
      html: "<p>" + ew.language.phrase("Exporting") + "</p>",
      allowOutsideClick: false,
      allowEscapeKey: false,
      willOpen: () => {
        swal.showLoading();
        _fetch(href, {
          method: data ? "POST" : "GET",
          body: data || null
        }).then(async response => {
          let ct = response.headers.get("Content-Type");
          if (ct != null && ct.includes("json")) {
            var _getError;
            let result = await response.json(),
              error = (_getError = getError(result)) != null ? _getError : ew.language.phrase("FailedToExport");
            if (error) {
              swal.hideLoading();
              swal.update({
                html: "<div class='text-danger'>" + error + "</div>",
                showConfirmButton: true
              });
              $document$1.trigger("export", [{
                type: "fail",
                url: href
              }]);
            }
          } else {
            let data = isHtml ? await response.text() : await response.blob(),
              url = URL.createObjectURL(isHtml ? new Blob([data], {
                type: "text/html"
              }) : data),
              a = document.createElement("a"),
              cd = response.headers.get("Content-Disposition"),
              m = cd == null ? void 0 : cd.match(/\bfilename=((['"])(.+)\2|([^;]+))/i);
            a.style.display = "none";
            a.href = url;
            if (m) a.download = m[3] || m[4];
            document.body.appendChild(a);
            a.click();
            $document$1.trigger("export", [{
              type: "done",
              url: href,
              objectUrl: url
            }]);
            URL.revokeObjectURL(url);
            swal.close();
          }
        }).catch(error => {
          swal.hideLoading();
          swal.update({
            html: "<div class='text-danger'>" + error + "</div>",
            showConfirmButton: true
          });
          $document$1.trigger("export", [{
            type: "fail",
            url: href
          }]);
        }).finally(() => {
          $document$1.trigger("export", [{
            type: "always",
            url: href
          }]);
        });
      }
    });
  }

  // Lazy load images
  function lazyLoad(e) {
    var _e$target5;
    if (!ew.LAZY_LOAD) return;
    var el = (_e$target5 = e == null ? void 0 : e.target) != null ? _e$target5 : document;
    el.querySelectorAll("img.ew-lazy").forEach((img, i) => {
      if (ew.LAZY_LOAD_DELAY > 0) setTimeout(() => img.src = img.dataset.src, i * ew.LAZY_LOAD_DELAY);else img.src = img.dataset.src;
    });
    $document$1.trigger("lazyload"); // All images loaded
  }

  // Update select2 dropdown position
  function updateDropdownPosition() {
    var select = $__default.default(".select2-container--open").prev(".ew-select").data("select2");
    if (select) {
      select.dropdown._positionDropdown();
      select.dropdown._resizeDropdown();
    }
  }

  // Colorboxes
  function initLightboxes(e) {
    var _e$target6;
    if (!ew.USE_COLORBOX) return;
    var el = (_e$target6 = e == null ? void 0 : e.target) != null ? _e$target6 : document;
    var settings = Object.assign({}, ew.lightboxSettings, {
      title: ew.language.phrase("LightboxTitle"),
      current: ew.language.phrase("LightboxCurrent"),
      previous: ew.language.phrase("LightboxPrevious"),
      next: ew.language.phrase("LightboxNext"),
      close: ew.language.phrase("LightboxClose"),
      xhrError: ew.language.phrase("LightboxXhrError"),
      imgError: ew.language.phrase("LightboxImgError")
    });
    $__default.default(el).find(".ew-lightbox").each(function () {
      var $this = $__default.default(this);
      $this.colorbox(Object.assign({
        rel: $this.data("rel")
      }, settings));
    });
  }

  // PDFObjects
  function initPdfObjects(e) {
    var _e$target7;
    if (!ew.EMBED_PDF) return;
    let el = (_e$target7 = e == null ? void 0 : e.target) != null ? _e$target7 : document,
      options = Object.assign({}, ew.PDFObjectOptions);
    $__default.default(el).find(".ew-pdfobject").not(":has(.pdfobject)").each(function () {
      // Not already embedded
      let $this = $__default.default(this),
        url = $this.data("url"),
        html = $this.html();
      if (url) PDFObject.embed(url, this, Object.assign(options, {
        fallbackLink: html
      }));
    });
  }

  // Tooltips and popovers
  function initTooltips(e) {
    var _e$target8;
    let el = (_e$target8 = e == null ? void 0 : e.target) != null ? _e$target8 : document,
      $el = $__default.default(el);
    $el.find("input[data-bs-toggle=tooltip], textarea[data-bs-toggle=tooltip], select[data-bs-toggle=tooltip]").each(function () {
      let $this = $__default.default(this);
      $this.tooltip({
        ...ew.tooltipOptions,
        html: true,
        ...$this.data()
      });
    });
    $el.find("a.ew-tooltip-link").each(tooltip); // Set up tooltip links as popovers
    $el.find(".ew-tooltip").tooltip(ew.tooltipOptions);
    $el.find(".ew-popover").popover(ew.popoverOptions);
  }

  // Parse JSON
  function parseJson(data) {
    if ($__default.default.isString(data)) {
      try {
        return JSON.parse(data);
      } catch (e) {
        return undefined;
      }
    }
    return data;
  }

  // Change search operator
  function searchOperatorChange(el) {
    let $el = $__default.default(el),
      $p = $el.closest("[id^=r_], [id^=xs_]"),
      parm = el.id.substring(2),
      $fld = $p.find(".ew-search-field"),
      $fld2 = $p.find(".ew-search-field2"),
      $y = $fld2.find("[name='y_" + parm + "'], [name='y_" + parm + "[]']"),
      hasY = $y.length,
      $cond = $p.find(".ew-search-cond"),
      hasCond = $cond.length,
      // Has condition and operator 2
      $and = $p.find(".ew-search-and"),
      $opr = $p.find(".ew-search-operator"),
      opr = $opr.find("[name='z_" + parm + "']").val(),
      $opr2 = $p.find(".ew-search-operator2"),
      opr2 = $opr2.find("[name='w_" + parm + "']").val(),
      isBetween = opr.includes("BETWEEN"),
      // Can only be operator 1
      isEmptyOpr = ["IS NULL", "IS NOT NULL", "IS EMPTY", "IS NOT EMPTY"].includes(opr),
      isEmptyOpr2 = ["IS NULL", "IS NOT NULL", "IS EMPTY", "IS NOT EMPTY"].includes(opr2),
      hideOpr2 = !hasY || isBetween,
      hideX = isEmptyOpr,
      hideY = !isBetween && (!hasCond || isEmptyOpr2);
    $cond.toggleClass("d-none", hideOpr2).find(":input").prop("disabled", hideOpr2);
    $and.toggleClass("d-none", !isBetween);
    $opr2.toggleClass("d-none", hideOpr2).find(":input").prop("disabled", hideOpr2);
    $fld.toggleClass("d-none", hideX).find(":input").prop("disabled", hideX);
    $fld2.toggleClass("d-none", hideY).find(":input").prop("disabled", hideY);
  }

  // Init forms
  function initForms(e) {
    var _e$target9;
    let el = (_e$target9 = e == null ? void 0 : e.target) != null ? _e$target9 : document,
      ids = ew.forms.ids();
    for (let id of ids) {
      if (el.matches != null && el.matches("#" + id) || el.querySelector("#" + id)) forms.get(id).init();
    }
  }

  // Is function
  function isFunction$2(x) {
    return typeof x === "function";
  }

  /**
   * Alert (OK button only)
   *
   * @param {string|Object} msg - Message or config
   * @param {callback} [cb] - Callback function
   * @param {string} [type] - CSS class (see https://getbootstrap.com/docs/5.2/utilities/colors/)
   * @returns {Promise}
   */
  function _alert(msg, cb, type) {
    let config = $__default.default.isObject(msg) ? msg : {};
    msg = $__default.default.isString(msg) ? msg : "";
    type = $__default.default.isString(cb) ? cb : type;
    config = $__default.default.extend(true, {}, ew.sweetAlertSettings, {
      html: msg,
      confirmButtonText: ew.language.phrase("OKBtn"),
      customClass: {
        htmlContainer: "ew-swal2-html-container text-" + (type || "danger")
      }
    }, config);
    let args = {
      config,
      type,
      show: true
    };
    $document$1.trigger("alert", [args]);
    if (args.show) return Swal.fire(args.config).then(result => isFunction$2(cb) ? cb(result.isConfirmed) : result);
  }

  /**
   * Prompt/Confirm/Alert
   *
   * @param {string|Object} cfg - Message or config object
   * @param {callback} [cb] - Callback function
   * @returns {Promise}
   */
  function _prompt(cfg, cb) {
    var _config, _config$inputValidato;
    let config = $__default.default.isObject(cfg) ? cfg : {};
    config = $__default.default.extend(true, {}, ew.sweetAlertSettings, {
      html: $__default.default.isString(cfg) ? cfg : "",
      showCancelButton: true,
      confirmButtonText: ew.language.phrase("OKBtn"),
      cancelButtonText: ew.language.phrase("CancelBtn")
    }, config); // Confirm/Alert
    if (config.input)
      // Prompt
      (_config$inputValidato = (_config = config).inputValidator) != null ? _config$inputValidato : _config.inputValidator = value => {
        if (!value) return ew.language.phrase("EnterValue");
      };
    return Swal.fire(config).then(result => isFunction$2(cb) ? cb(result.value) : result);
  }

  // Toast
  function toast(options) {
    options = Object.assign({}, ew.toastOptions, options);
    $document$1.Toasts("create", options);
    var position = options.position,
      $container = $__default.default("#toastsContainer" + position[0].toUpperCase() + position.substring(1));
    return $container.children().first();
  }

  /**
   * Show toast
   *
   * @param {string} message - Message
   * @param {string} type - CSS class: "primary|secondary|success|info|warning|danger|light|dark"
   * @param {string} title - Title
   */
  function showToast(message, type, title) {
    if (!message) return;
    type || (type = "danger");
    title || (title = ew.language.phrase(type));
    let args = {
      message,
      type,
      title,
      show: true
    };
    $document$1.trigger("toast", [args]);
    if (!args.show) return;
    ({
      message,
      type,
      title
    } = args);
    return toast({
      class: "ew-toast bg-" + type,
      title,
      body: message,
      autohide: type == "success" ? ew.autoHideSuccessMessage : false,
      // Autohide for success message
      delay: type == "success" ? ew.autoHideSuccessMessageDelay : 500
    });
  }

  // Get form.ew-form or div.ew-form HTML element
  function getForm(el) {
    if (el instanceof Form) return el.$element[0];
    var $el = $__default.default(el),
      $f = $el.closest(".ew-form");
    if (!$f[0])
      // Element not inside form
      $f = $el.closest(".ew-grid, .ew-multi-column-grid").find(".ew-form");
    return $f[0];
  }

  // Check form data
  function hasFormData(form) {
    var selector = "[name^=x_],[name^=y_],[name^=z_],[name^=w_],[name=" + ew.TABLE_BASIC_SEARCH + "]",
      els = $__default.default(form).find(selector).filter(":enabled").get();
    for (var i = 0, len = els.length; i < len; i++) {
      var el = els[i];
      if (/^(z|w)_/.test(el.name)) {
        if (/^IS/.test($__default.default(el).val())) return true;
      } else if (el.type == "checkbox" || el.type == "radio") {
        if (el.checked) return true;
      } else if (el.type == "select-one" || el.type == "select-multiple") {
        if (!!$__default.default(el).val()) return true;
      } else if (["text", "textarea", "password", "search", "color", "date", "datetime-local", "datetime", "email", "hidden", "month", "number", "range", "tel", "time", "url", "week"].includes(el.type)) {
        if (el.value) return true;
      }
    }
    return false;
  }

  /**
   * Set search type
   *
   * @param {HTMLElement} el - HTML element
   * @returns false
   */
  function setSearchType(el) {
    var val = el.dataset.searchType,
      phraseId = "Auto";
    if (val == "=") phraseId = "Exact";else if (val == "AND") phraseId = "All";else if (val == "OR") phraseId = "Any";
    el.closest(".ew-basic-search").querySelector("input.ew-basic-search-type").value = val || "";
    el.closest(".dropdown-menu").querySelectorAll(".dropdown-item").forEach(item => item.classList.remove("active"));
    el.closest(".dropdown-item").classList.add("active");
    var searchType = el.closest(".input-group").querySelector("#searchtype"),
      text = ew.language.phrase("QuickSearch" + phraseId + "Short");
    searchType.innerHTML = text;
    searchType.classList.toggle("me-2", !!text);
    return false;
  }

  /**
   * Update a dynamic selection list
   *
   * @this {Form|HTMLElement} Form or parent element
   * @param {(HTMLElement|HTMLElement[]|string|string[])} obj - Target HTML element(s) or the ID of the element(s)
   * @param {(string[]|array[])} parentId - Parent field element names or data
   * @param {(boolean|null)} async - async(true) or sync(false) or non-Ajax(null)
   * @param {boolean} change - Trigger onchange event
   * @returns {Promise}
   */
  function updateOptions(obj, parentId, async, change) {
    var _obj, _batch$send;
    var f = this.$element ? this.$element[0] : this.form || ((_obj = obj) == null ? void 0 : _obj.form); // Get form/div element from this/obj
    if (!f) return;
    var frm = this.htmlForm ? this : forms.get(f.id); // Get Form object
    if (!frm) return;
    if (this.form && $__default.default.isUndefined(obj))
      // Target unspecified => update child fields
      obj = forms.get(this).getList(this).childFields.slice(); // Clone
    else if ($__default.default.isString(obj)) obj = getElements(obj, f);
    if (!obj || Array.isArray(obj) && obj.length == 0) return;
    var self = this,
      batch = new Batch();
    if (Array.isArray(obj) && $__default.default.isString(obj[0])) {
      // Array of id
      var els = [];
      for (var i = 0, len = obj.length; i < len; i++) {
        var ar = obj[i].split(" ");
        if (ar.length == 1 && self.form) {
          // Parent/Child fields in the same table
          var m = getId(self, false).match(/^([xy]\d*_)/);
          if (m) obj[i] = obj[i].replace(/^([xy]\d*_)/, m[1]);
        }
        var el = getElements(obj[i], f),
          names = [];
        if (isTextbox(el) || isFilter(el))
          // Search text box or filter
          continue;
        els.push(el);
        if (ar.length == 2 && Array.isArray(el)) {
          // Check if id is "tblVar fldVar" and multiple inputs
          var $el = $__default.default(el);
          $el.each(function () {
            if (!names.includes(this.name)) {
              names.push(this.name);
              var $elf = $el.filter("[name='" + this.name + "']"),
                typ = $elf.attr("type"),
                elf = ["radio", "checkbox"].includes(typ) ? $elf.get() : $elf[0];
              batch.add(_updateOptions.bind(self, elf, parentId, async, change));
            }
          });
        } else {
          batch.add(_updateOptions.bind(self, el, parentId, async, change));
        }
      }
      obj = els;
      var list = forms.get(self).getList(self);
      if (Array.isArray(list == null ? void 0 : list.autoFillTargetFields) && list.autoFillTargetFields[0])
        // AutoFill
        batch.add(autoFill.bind(null, self));
    } else {
      if (isTextbox(obj) || isFilter(obj))
        // Search text box or filter
        return;
      batch.add(_updateOptions.bind(self, obj, parentId, async, change));
    }
    return (_batch$send = batch.send({
      url: ew.getApiUrl(ew.API_LOOKUP_ACTION)
    })) == null ? void 0 : _batch$send.then(function () {
      $document$1.trigger("updatedone", [{
        source: self,
        target: obj
      }]); // Document "updatedone" event fired after all the target elements are updated
    });
  }

  /**
   * Update a dynamic selection list
   *
   * @param {(HTMLElement|HTMLElement[]} obj - Target HTML element(s) or the ID of the element(s)
   * @param {(string[]|array[])} parentId - Parent field element names or data
   * @param {(boolean|null)} async - async(true) or sync(false) or non-Ajax(null)
   * @param {boolean} change - Trigger onchange event
   * @returns {Promise}
   */
  function _updateOptions(obj, parentId, async, change) {
    if (change === void 0) {
      change = true;
    }
    var id = getId(obj, false);
    if (!id) return;
    var fo = getForm(obj); // Get form/div element from obj
    if (!fo || !fo.id) return;
    var frmo = forms.get(fo.id);
    if (!frmo) return;
    var self = this,
      args = Array.from(arguments),
      ar = getOptionValues(obj),
      m = id.match(/^([xy])(\d*)_/),
      prefix = m ? m[1] : "",
      rowindex = m ? m[2] : "",
      arp = [],
      list = frmo.getList(obj),
      $obj = $__default.default(obj).data("updating", true);
    if ($obj.data("hidden"))
      // Skip data-hidden field, e.g. detail key
      return;
    if ($__default.default.isUndefined(parentId)) {
      // Parent IDs not specified, use default
      parentId = list.parentFields.slice(); // Clone
      if (rowindex != "") {
        for (var i = 0, len = parentId.length; i < len; i++) {
          var arr = parentId[i].split(" ");
          if (arr.length == 1)
            // Parent field in the same table, add row index
            parentId[i] = parentId[i].replace(/^x_/, "x" + rowindex + "_");
        }
      }
    }
    if (Array.isArray(parentId) && parentId.length > 0) {
      if (Array.isArray(parentId[0])) {
        // Array of array => data
        arp = parentId;
      } else if ($__default.default.isString(parentId[0])) {
        // Array of string => Parent IDs
        for (var i = 0, len = parentId.length; i < len; i++) arp.push(getOptionValues(parentId[i], fo));
      }
    }
    if (!isAutoSuggest(obj))
      // Do not clear Auto-Suggest
      clearOptions(obj);
    var addOpt = function (results) {
      var name = getId(obj);
      results.forEach(function (result) {
        let args = {
          "data": result,
          "parents": arp,
          "valid": true,
          "name": name,
          "form": fo
        };
        $document$1.trigger("addoption", [args]);
        if (args.valid) newOption(obj, result, fo);
      });
      obj.render == null ? void 0 : obj.render(); // Selection list
      selectOption(obj, ar, change);
      if (change) {
        if (!obj.options && obj.length) $obj.first().triggerHandler("click");
        // else
        //     $obj.first().trigger("change");
      }
    };

    if ($__default.default.isUndefined(async))
      // Async not specified, use default
      async = list.ajax;
    var _updateSibling = function () {
      // Update the y_* element
      if (/(srch|search|summary|crosstab)$/.test(fo.id) && prefix == "x" && !rowindex) {
        // Search form
        args[0] = id.replace(/^x_/, "y_");
        updateOptions.apply(self, args); // args[0] is string, use updateOptions()
      }
    };

    if (!$__default.default.isBoolean(async) || Array.isArray(list.lookupOptions) && list.lookupOptions.length > 0) {
      // Non-Ajax or Options loaded
      var ds = list.lookupOptions;
      addOpt(ds);
      _updateSibling();
      $obj.first().trigger("updated", [{
        target: obj
      }]); // Object "updated" event fired after the object is updated
      return ds;
    } else {
      // Ajax
      var name = getId(obj),
        data = Object.assign({
          page: list.page,
          field: list.field,
          ajax: "updateoption",
          language: ew.LANGUAGE_ID,
          name: name // Name of the target element
        }, getUserParams("#p_" + id, fo)); // Add user parameters
      if (isAutoSuggest(obj) && self.htmlForm)
        // Auto-Suggest (init form or auto-fill)
        data["v0"] = ar[0] || random(); // Filter by the current value
      else if (obj.options && !obj.list && !isNativeSelectOne(obj) ||
      // Not <selection-list> or native <select>
      isModalLookup(obj))
        // Lookup
        data["v0"] = ar[0] ? obj.multiple ? ar.join(ew.MULTIPLE_OPTION_SEPARATOR) : ar[0] : random(); // Filter by the current value
      for (var i = 0, cnt = arp.length; i < cnt; i++)
      // Filter by parent fields
      data["v" + (i + 1)] = arp[i].join(ew.MULTIPLE_OPTION_SEPARATOR);
      obj.showLoading == null ? void 0 : obj.showLoading(); // selection-list
      return $__default.default.ajax(getApiUrl(ew.API_LOOKUP_ACTION), {
        method: "POST",
        dataType: "json",
        data: data,
        async: async,
        processData: false,
        success: result => {
          let error = getError(result);
          if (error) {
            _alert(error);
            return;
          }
          let ds = result.records || [];
          addOpt(ds);
          _updateSibling();
          $obj.first().trigger("updated", [Object.assign({}, result, {
            target: obj
          })]); // Object "updated" event fired after the object is updated
          return ds;
        },
        error: (jqXHR, textStatus, errorThrown) => _alert(errorThrown),
        complete: () => $obj.data("updating", false)
      });
    }
  }

  // Get user parameters from id
  function getUserParams(id, root) {
    var id = id.replace(/\[\]$/, ""),
      o = {};
    var root = !$__default.default.isString(root) ? root : /^#/.test(root) ? root : "#" + root;
    var $els = root ? $__default.default(root).find(id) : $__default.default(id);
    var val = $els.val();
    if (val) {
      var params = new URLSearchParams(val);
      params.forEach(function (value, key) {
        o[key] = value;
      });
    }
    return o;
  }

  // Get query builder filter input HTML
  function getQueryBuilderFilterInput() {
    return function (rule, name) {
      var _document$getElementB, _document$getElementB2, _span$innerHTML$repla;
      let tblVar = this.status.id.replace(/_query_builder$/, ""),
        // "this" is QueryBuilder
        fldVar = rule.filter.id,
        tmplId = "tpx_" + tblVar + "_" + fldVar,
        span = (_document$getElementB = document.getElementById(tmplId)) == null ? void 0 : (_document$getElementB2 = _document$getElementB.content) == null ? void 0 : _document$getElementB2.cloneNode(true).querySelector(".ew-search-field"),
        html = (_span$innerHTML$repla = span == null ? void 0 : span.innerHTML.replace(/\binvalid-feedback\b/, ew.queryBuilderErrorClass).replace(new RegExp('(data-field=")?(\\b|_)x_' + fldVar + '\\b', "g"), ($0, $1, $2) => $1 ? $0 : $2 + name)) != null ? _span$innerHTML$repla : ""; // Keep "data-field" attribute
      return "<div class=\"d-inline-flex position-relative\">" + html + "</div>";
    };
  }

  // Get query builder value setter
  function getQueryBuilderValueSetter() {
    return function (rule, value) {
      let input = rule.$el.find(".rule-value-container").find("select, selection-list, input[name^=sv_], input[data-field]")[0],
        values = Array.isArray(value) ? value : $__default.default.isValue(value) ? [value] : [];
      if (input != null && input.matches("select, selection-list")) {
        // Selection list
        let form = forms.get(input);
        values.forEach(val => newOption(input, {
          lf: val
        }, form, true));
        form.updateList(input, undefined, undefined, true); // Update immediately
      } else {
        ew.selectOption(input, values);
      }
      return null;
    };
  }

  // Get query builder filter validation object
  function getQueryBuilderFilterValidation(validators) {
    return {
      callback: function (value, rule) {
        // value not used
        let $value = rule.$el.find(".rule-value-container"),
          errors = 0;
        for (let i = 0; i < rule.operator.nb_inputs; i++) {
          $value.find("[name='" + rule.id + '_value_' + i + "']").each(function () {
            let $this = $__default.default(this),
              $invalid = $this.next("." + ew.queryBuilderErrorClass).empty();
            validators == null ? void 0 : validators.forEach == null ? void 0 : validators.forEach(validator => {
              let err = validator(this);
              if ($__default.default.isObject(err)) {
                errors++;
                $invalid.append("<span>" + Object.values(err)[0] + "</span>");
              }
            });
            if ($invalid.html()) $this.addClass("is-invalid").one("click", () => $this.removeClass("is-invalid"));
          });
        }
        return errors ? "" : true; // Return error as empty string (errors already set in $invalid)
      }
    };
  }

  // Apply client side template to a DIV
  function applyTemplate(divId, tmplId, classId, exportType, tblVar, data, isModal) {
    let args = {
      data: data || {},
      id: divId,
      template: tmplId,
      class: classId,
      export: exportType,
      enabled: true
    };
    $document$1.trigger("rendertemplate", [args]);
    if (args.enabled) {
      var _parent$querySelector;
      let parent = isModal ? document.querySelector("#ew-modal-dialog.show") : document.body,
        // Parent (Modal dialog or document)
        template = parent == null ? void 0 : (_parent$querySelector = parent.querySelector("#" + tmplId)) == null ? void 0 : _parent$querySelector.content;
      if (!template) return;
      template.querySelectorAll(".ew-slot").forEach(el => {
        let id = el.name || el.id,
          subtmpl = parent.querySelector("#" + id); // Find in parent in case Custom Template in modal dialog
        if (subtmpl != null && subtmpl.content) {
          if (el.dataset.rowspan > 1) Array.prototype.slice.call(subtmpl.content.childNodes).forEach(node => node.rowSpan = el.dataset.rowspan);
          el.replaceWith(subtmpl.content.cloneNode(true));
        } else {
          el.remove();
        }
      });
      if ($__default.default.views) {
        let textContent = template.textContent,
          hasTag = textContent.includes("{{") && textContent.includes("}}");
        if (!hasTag) {
          let selector = ew.jsRenderAttributes.map(attr => "[" + attr + "*='{{'][" + attr + "*='}}']").join(",");
          hasTag = template.querySelector(selector);
        }
        if (hasTag) {
          // Includes JsRender template
          let scripts = Array.prototype.slice.call(template.querySelectorAll("script")); // Extract scripts
          scripts.forEach(item => item.remove());
          let div = document.createElement("div");
          div.appendChild(template);
          let html = div.innerHTML.replace(/{{([^}]+)}}/g, m => htmlDecode(m)),
            // HTML-decode comparison operators
            tmpl = $__default.default.templates(html);
          parent.querySelector("#" + divId).innerHTML = tmpl.render(args.data, ew.jsRenderHelpers);
          scripts.forEach(item => document.body.appendChild(item)); // Add scripts
        } else {
          parent.querySelector("#" + divId).appendChild(template);
        }
      } else {
        parent.querySelector("#" + divId).appendChild(template);
      }
    }
    if (exportType && exportType != "print")
      // Export custom template with charts
      exportCustom(divId, exportType, tblVar); // Note: Use classId as fileName
  }

  // Export custom
  function exportCustom(divId, exportType, tblVar) {
    var _div$children$;
    let div = document.getElementById(divId);
    if ((_div$children$ = div.children[0]) != null && _div$children$.matches("div[id^=ct_]"))
      // Custom template, remove first div tag
      div = div.children[0];
    let data = {
      customexport: exportType,
      data: div.innerHTML,
      [ew.TOKEN_NAME_KEY]: ew.TOKEN_NAME,
      [ew.ANTIFORGERY_TOKEN_KEY]: ew.ANTIFORGERY_TOKEN,
      ...getchartParams()
    };
    if (exportType == "email") exportEmail(tblVar, mergeSearchParams(currentUrl.searchParams, data)); // Merge data with email form data in current URL parameters
    else fileDownload(getApiUrl([ew.API_EXPORT_ACTION, exportType, tblVar]), data);
  }

  // Toggle group
  function toggleGroup(el) {
    var $el = $__default.default(el),
      $tr = $el.closest("tr"),
      selector = "tr",
      level;
    for (var i = 1; i <= 6; i++) {
      var idx = i == 1 ? "" : "-" + i;
      var data = $tr.data("group" + idx);
      if ($__default.default.isValue(data)) {
        level = i;
        if (data != "") selector += "[data-group" + idx + "='" + String(data).replace(/'/g, "\\'") + "']";
      }
    }
    if ($el.hasClass("ew-rpt-grp-hide")) {
      // Show
      $__default.default(selector).slice(1).removeClass("ew-rpt-grp-hide-" + level);
      $el.removeClass("ew-rpt-grp-hide");
    } else {
      // Hide
      $__default.default(selector).slice(1).addClass("ew-rpt-grp-hide-" + level);
      $el.addClass("ew-rpt-grp-hide");
    }
  }

  // Check if boolean value is true
  function convertToBool(value) {
    return value && ["1", "y", "t", "true"].includes(String(value).toLowerCase());
  }

  // Check if element value changed
  function valueChanged(fobj, infix, fld, bool) {
    let el = getElements("x" + infix + "_" + fld, fobj),
      oldEl = getElement("o" + infix + "_" + fld, fobj),
      // Hidden element
      fnEl = getElement("fn_x" + infix + "_" + fld, fobj); // Hidden element
    if ((el == null ? void 0 : el.type) == "hidden" && !oldEl)
      // For example, detail key
      return false;
    if (!oldEl && (!el || Array.isArray(el) && el.length == 0)) return false;
    if (oldEl && el) {
      if (bool) {
        if (convertToBool(getOptionValue(oldEl)) === convertToBool(getOptionValue(el))) return false;
      } else {
        let oldValue = getOptionValue(oldEl),
          newValue = fnEl ? getOptionValue(fnEl) : getOptionValue(el);
        if (oldValue == newValue) return false;
      }
    }
    return true;
  }

  // Set language
  function setLanguage(el) {
    let val = el.value || el.dataset.language;
    if (!val) return false;
    let currentUrl = new URL(window.location);
    currentUrl.searchParams.set("language", val);
    window.location = sanitizeUrl(currentUrl.toString());
    return false;
  }

  /**
   * Submit action
   *
   * @param {MouseEvent} e - Mouse event
   * @param {Object} args - Arguments
   * @param {HTMLElement} args.f - HTML form (default is the form of the source element) (for backward compatibility only)
   * @param {string} args.url - URL to which the request is sent (default is current page) (for backward compatibility only)
   * @param {Object} args.key - Key as object (for single record only)
   * @param {string|Object} args.msg - Message or Swal config
   * @param {string} args.action - Custom action name
   * @param {string} args.select - "single"|"s" (single record) or "multiple"|"m" (multiple records, default)
   * @param {string} args.method - "ajax"|"a" (Ajax by HTTP POST) or "post"|"p" (HTTP POST by HTML form, default)
   * @param {Object} args.data - Object of user data that is sent to the server
   * @param {string|callback|Object} success - Function to be called if the request succeeds, or settings for jQuery.ajax() (for Ajax only)
   * @returns
   */
  function submitAction(e, args) {
    var _window$msg;
    let el = e.currentTarget,
      f = args.f || el.form || el.closest("form"),
      $f = $__default.default(f),
      key = args.key,
      action = args.action,
      url = args.url || currentPage(),
      msg = args.msg,
      data = args.data,
      success = args.success,
      isPostBack = !args.method || sameText(args.method[0], "p"),
      isMultiple = !args.select && !args.key || args.select && sameText(args.select[0], "m");
    if ((isMultiple || isPostBack) && !f) {
      _alert(ew.language.phrase("NoHtmlForm"));
      return false;
    }
    if (isMultiple && !keySelected(f)) {
      _alert(ew.language.phrase("NoRecordSelected"));
      return false;
    }
    let _success = function (result) {
      showMessage(result);
    };
    let _append = function (name, value) {
      let $input = $f.find("input[type=hidden][name='" + name + "']");
      if ($input[0])
        // Hidden tag exists
        $input.val(value); // Set value
      else
        // Hidden tag does not exist, add one
        $__default.default("<input>").attr({
          type: "hidden",
          name,
          value
        }).appendTo($f);
    };
    let _submit = function (value) {
      if (isPostBack && f) {
        // Post back by form
        if (action) {
          // Action
          _append("action", action); // Set action
        }

        if (!$__default.default.isUndefined(value)) _append("actionvalue", value); // Set action value
        if ($__default.default.isObject(data)) {
          // User data
          for (const [name, value] of Object.entries(data)) _append(name, value);
        }
        if (!isMultiple && $__default.default.isObject(key)) {
          // Key
          for (const [name, value] of Object.entries(key)) _append(name, value);
        }
        if (parseUrl(url).pathname != currentUrl.pathname)
          // Do not use refresh if post to another page
          $f.off("submit.ew");
        $f.prop({
          action: url,
          method: "post"
        }).trigger("submit");
      } else {
        // Ajax
        data = new URLSearchParams(data); // User data
        if (action) {
          // Action
          data.set("action", action);
          data.set("ajax", action);
          $f.find("#action").remove(); // Remove action in form
        }

        if (!$__default.default.isUndefined(value)) data.set("actionvalue", value); // User input value
        data = mergeSearchParams(data, $f.serialize()); // Form data including key_m[]
        if (!isMultiple && $__default.default.isObject(key))
          // Key
          data = mergeSearchParams(data, key);
        if (success && $__default.default.isString(success)) success = window[success];
        let settings = $__default.default.isObject(success) ? success : {};
        settings.data = data.toString();
        settings.method || (settings.method = "POST");
        settings.success || (settings.success = isFunction$2(success) ? success : _success);
        $__default.default.ajax(url, settings);
      }
    };
    msg = $__default.default.isString(msg) ? (_window$msg = window[msg]) != null ? _window$msg : msg : msg; // Get config object if available
    msg ? _prompt(msg).then(result => result.isDismissed || _submit(result.value)) : _submit();
    return false;
  }

  /**
   * Get charts parameters as object
   *
   * @returns string
   */
  function getchartParams() {
    let charts = Array.from(Object.entries(window.exportCharts)).map(_ref => {
      let [id, chart] = _ref;
      return {
        chartEngine: "Chart.js",
        streamType: "base64",
        stream: chart.toBase64Image(),
        fileName: id + ".png"
      };
    });
    return charts.length ? {
      charts: JSON.stringify(charts)
    } : null;
  }

  /**
   * Export with charts, selected records and/or Custom Template (non-API)
   *
   * @param {MouseEvent|HTMLFormElement} args.evt - Event or HTML form
   * @param {string} args.url - Form action
   * @param {string} args.export - Export type
   * @param {boolean} args.custom - Use Custom Template
   * @param {boolean} args.exportSelected - Selected records only
   * @param {HTMLFormElement} args.emailForm - email form object
   * @returns false
   */
  async function _export(args) {
    let {
        evt,
        url,
        export: type,
        custom,
        exportSelected,
        emailForm
      } = args,
      f = evt.currentTarget.form;
    exportSelected && (exportSelected = f && !!f.querySelector("input[type=checkbox][name='key_m[]']"));
    if (exportSelected && !keySelected(f)) {
      _alert(ew.language.phrase("NoRecordSelected"));
      return false;
    }
    let $f = f ? $__default.default(f) : $__default.default('<form class="ew-export-form" method="post"></form>').appendTo($body),
      target = $f.attr("target"),
      action = $f.attr("action"),
      postParams = [[ew.TOKEN_NAME_KEY, ew.TOKEN_NAME], [ew.ANTIFORGERY_TOKEN_KEY, ew.ANTIFORGERY_TOKEN]];

    // Export data
    try {
      if (custom) {
        var _document$querySelect;
        // Use Custom Template (chart data to be retrieved later in iframe)
        document.body.style.cursor = "wait";
        let name = "ew-export-frame";
        url = setSearchParams(url, {
          export: type,
          custom: 1
        }, $__default.default(emailForm).serialize()); // Make sure parameters include "export=type&custom=1"
        (_document$querySelect = document.querySelector("iframe[name='" + name + "']")) == null ? void 0 : _document$querySelect.remove();
        let $iframe = $__default.default("<iframe>").attr("name", name).addClass("d-none").appendTo($body);
        $f.attr({
          action: url,
          target: name
        }); // Pass all data including email form data from URL to the iframe by URL parameters
        postParams.forEach(_ref2 => {
          let [name, value] = _ref2;
          return $f.find("input[name=\"" + name + "\"]").val(value)[0] || $__default.default("<input>").attr({
            type: "hidden",
            name,
            value
          }).appendTo($f);
        });
        $iframe.one("load", () => document.body.style.cursor = "default").one("load", () => $f.attr({
          action,
          target
        })); // Reset
        $f.trigger("submit");
      } else {
        // No Custom Template
        url = setSearchParam(url, "export", type); // Make sure URL parameters include "export=type"
        if (type == "print") $f.attr("action", url).trigger("submit"); // Submit the form directly
        else fileDownload(url, mergeSearchParams($f.serialize(), getchartParams()));
      }
    } finally {
      f || $f.remove();
    }
    return false;
  }

  /**
   * Remove spaces
   * @param {string} value - Value
   * @returns {string}
   */
  function removeSpaces(value) {
    return /^(<(p|br)\/?>(&nbsp;)?(<\/p>)?)?$/i.test(value.replace(/\s/g, "")) ? "" : value;
  }

  /**
   * Check if hidden text area (HTML editor)
   * @param {HTMLElement|jQuery} el - HTML element or jQuery object
   * @returns {boolean}
   */
  function isHiddenTextArea(el) {
    var $el = $__default.default(el);
    return $el.is(":hidden") && $el.data("editor");
  }

  /**
   * Check if modal lookup
   * @param {HTMLElement} el - HTML element
   * @returns {boolean}
   */
  function isModalLookup(el) {
    var _el$dataset;
    return el == null ? void 0 : (_el$dataset = el.dataset) == null ? void 0 : _el$dataset.modalLookup;
  }

  /**
   * Check if filter
   * @param {HTMLElement} el - HTML element
   * @returns {boolean}
   */
  function isFilter(el) {
    var _el$dataset2;
    return el == null ? void 0 : (_el$dataset2 = el.dataset) == null ? void 0 : _el$dataset2.filter;
  }

  /**
   * Check if hidden textbox (Auto-Suggest)
   * @param {HTMLElement|jQuery} el - HTML element or jQuery object
   * @returns {boolean}
   */
  function isAutoSuggest(el) {
    var $el = $__default.default(el);
    return $el.is(":hidden") && $el.data("autosuggest");
  }

  /**
   * Check if Select2
   * @param {HTMLElement} el - HTML element
   * @returns {boolean}
   */
  function isSelect2(el) {
    return el == null ? void 0 : el.matches == null ? void 0 : el.matches("select.select2-hidden-accessible");
  }

  /**
   * Check if native selection list (select-one)
   * @param {HTMLElement} el - HTML element
   * @returns {boolean}
   */
  function isNativeSelectOne(el) {
    return (el == null ? void 0 : el.matches == null ? void 0 : el.matches("select:not(.select2-hidden-accessible)")) && el.type == "select-one";
  }

  /**
   * Check if textbox
   * @param {HTMLElement} el - HTML element
   * @returns {boolean}
   */
  function isTextbox(el) {
    return (el == null ? void 0 : el.matches == null ? void 0 : el.matches("input:not([type=checkbox]):not([type=radio])")) && !isAutoSuggest(el);
  }

  /**
   * Check if boolean checkbox
   * @param {HTMLElement} el - HTML element
   * @returns {boolean}
   */
  function isBooleanCheckbox(el) {
    return el == null ? void 0 : el.matches == null ? void 0 : el.matches("input[type=checkbox][data-boolean]");
  }

  /**
   * Clear error message
   * @param {HTMLElement|HTMLElement[]|jQuery} el - HTML element(s) or jQuery
   */
  function clearError(el) {
    if (el.jquery) {
      // el is jQuery object
      let typ = el.attr("type");
      el = typ == "checkbox" || typ == "radio" ? el.get() : el[0];
    }
    $__default.default(el).closest(fieldContainerSelector).find(".invalid-feedback").html("");
  }

  /**
   * Show error message
   * @param {Form} frm Form object
   * @param {HTMLElement|HTMLElement[]|jQuery} el - HTML element(s) or jQuery
   * @param {string} msg - Error message
   * @param {boolean} focus - Set focus
   */
  function onError(frm, el, msg, focus) {
    if (el.jquery) {
      // el is jQuery object
      let typ = el.attr("type");
      el = typ == "checkbox" || typ == "radio" ? el.get() : el[0];
    } else if (el instanceof Field) {
      // el is Field object
      el = el.element;
    }
    $__default.default(el).closest(fieldContainerSelector).find(".invalid-feedback").append("<p>" + msg + "</p>");
    if (focus) setFocus(el);
    frm == null ? void 0 : frm.makeVisible(el);
    return false;
  }

  /**
   * Set focus
   * @param {HTMLElement|HTMLElement[]} obj - HTML element(s)
   * @param {Object} options - Focus options
   */
  function setFocus(obj, options) {
    if (!obj) return;
    var $obj = $__default.default(obj);
    if (isHidden($obj)) return;
    if (isHiddenTextArea(obj)) {
      // HTML editor
      return $obj.data("editor").focus();
    } else if (!obj.options && obj.length) {
      // Radio/Checkbox list
      obj = $obj[0];
    } else if (isAutoSuggest(obj)) {
      // Auto-Suggest
      obj = obj.input;
    }
    obj.focus(options);
  }

  /**
   * Set invalid
   * @param {HTMLElement|HTMLElement[]} obj - HTML element(s)
   */
  function setInvalid(obj) {
    if (!obj) return;
    let $obj = $__default.default(obj);
    if (isHidden($obj)) return;
    if (!obj.options && obj.length)
      // Radio/Checkbox list
      obj = $obj[0];
    let $p = $obj.closest(fieldContainerSelector),
      reset = () => $p.find(".is-invalid").removeClass("is-invalid");
    if (isAutoSuggest(obj)) {
      $p.find(".ew-auto-suggest").removeClass("is-valid").addClass("is-invalid").one("click keydown paste", reset);
    } else if (isHiddenTextArea(obj)) {
      $obj.removeClass("is-valid").addClass("is-invalid");
      $obj.data("editor").instance.once("change", reset);
    } else if (isModalLookup(obj)) {
      $obj.removeClass("is-valid").addClass("is-invalid").one("select2:open", reset);
    } else {
      if (["checkbox", "radio"].includes(obj.type)) {
        $obj.removeClass("is-valid").addClass("is-invalid").one("click keydown", reset);
      } else {
        let events = ew.MOBILE_DETECT.version("Safari") && !ew.MOBILE_DETECT.version("Chrome") ? "change keydown paste" : "click keydown paste"; // Use "change" event for Safari
        $obj.parent().one(events, reset);
        $obj.add($obj.closest(".input-group")).removeClass("is-valid").addClass("is-invalid");
      }
    }
  }

  /**
   * Set valid
   * @param {HTMLElement|HTMLElement[]} obj - HTML element(s)
   */
  function setValid(obj) {
    if (!obj) return;
    let $obj = $__default.default(obj);
    if (isHidden($obj)) return;
    if (!obj.options && obj.length)
      // Radio/Checkbox list
      obj = $obj[0];
    let $p = $obj.closest(fieldContainerSelector),
      reset = () => $p.find(".is-valid").removeClass("is-valid");
    if (isAutoSuggest(obj)) {
      $p.find(".ew-auto-suggest").removeClass("is-invalid").addClass("is-valid").one("click keydown paste", reset);
    } else if (isHiddenTextArea(obj)) {
      $obj.removeClass("is-invalid").addClass("is-valid");
      $obj.data("editor").instance.once("change", reset);
    } else if (isModalLookup(obj)) {
      $obj.removeClass("is-invalid").addClass("is-valid").one("select2:open", reset);
    } else {
      if (["checkbox", "radio"].includes(obj.type)) {
        $obj.removeClass("is-invalid").addClass("is-valid").one("click keydown", reset);
      } else {
        let events = ew.MOBILE_DETECT.version("Safari") && !ew.MOBILE_DETECT.version("Chrome") ? "change keydown paste" : "click keydown paste"; // Use "change" event for Safari
        $obj.parent().one(events, reset);
        $obj.add($obj.closest(".input-group")).removeClass("is-invalid").addClass("is-valid");
      }
    }
  }

  // Check if object has value
  function hasValue(obj) {
    return getOptionValue(obj) != "";
  }

  // Check if object value is a masked password
  function isMaskedPassword(obj) {
    var val = $__default.default(obj).val();
    return val == null ? void 0 : val.match(/^\*+$/);
  }

  // Sort by field
  function sort(e, dataset) {
    let {
      sortUrl: url,
      sortType: type,
      context,
      ajax
    } = dataset;
    if (e.shiftKey && !e.ctrlKey) url = setSearchParams(url, {
      cmd: "resetsort"
    });else if (type == 2 && e.ctrlKey) url = setSearchParams(url, {
      ctrl: "1"
    });
    if (convertToBool(ajax)) refresh(fetch(setLayout(url, false)), context);else window.location = sanitizeUrl(url);
    return false;
  }

  // Open table header filter by field
  function filter(e) {
    let data = e.currentTarget.dataset;
    $__default.default("select[data-select2-id='f" + data.table + "srch_" + data.field + "']").select2("open");
  }

  /**
   * Scroll an element into view
   */
  function scrollIntoView(el) {
    var _OverlayScrollbars;
    let container = el == null ? void 0 : el.closest(".os-host"); // Check OverlayScrollbars
    container ? (_OverlayScrollbars = OverlayScrollbars(container)) == null ? void 0 : _OverlayScrollbars.scroll(el) : el == null ? void 0 : el.scrollIntoView();
  }

  /**
   * Inline actions
   *
   * @param {Object} args - Arguments
   * @param {MouseEvent} args.evt - Event
   * @param {HTMLFormElement} args.f - Form of List page
   * @param {string} args.url - URL
   * @param {string} args.ewAction - Inline/Grid action (inline/grid)
   * @param {string} args.action - Inline/Grid action type (insert/cancel/update/add/edit/copy/delete)
   * @returns false
   */
  async function inlineAction(args) {
    var _args$evt;
    let $el = $__default.default((_args$evt = args.evt) == null ? void 0 : _args$evt.currentTarget),
      $grid = $el.closest(".ew-grid, .ew-multi-column-grid");
    $el.tooltip("hide");
    if (!$grid[0] && $el.is(".dropdown-item")) {
      let $btn = $__default.default("#" + $el.closest(".dropdown-menu").attr("aria-labelledby"));
      if ($btn[0]) {
        $el = $btn;
        $grid = $el.closest(".ew-grid, .ew-multi-column-grid");
      }
    }
    if (!$el[0]) return;
    let {
        url,
        ewAction,
        action
      } = args,
      $f = $grid.find("form"),
      f = $f[0],
      $record = $el.closest("tr[data-rowindex], div[data-rowindex]"),
      key = $record.data("key"),
      curindex = $record.data("rowindex"),
      $detached;
    if (action != "add" && (!$grid[0] || !f))
      // Allow add with no records => no grid
      return;

    // Fail
    let _fail = function (o) {
      !o.status || showToast("Server Error " + o.status + ": " + o.statusText);
    };

    // Always
    let _always = function () {
      document.body.style.cursor = "default";
    };

    // Success handler for delete
    let _deleted = async function (response) {
      if (!(response instanceof Response))
        // Inline actions only
        return;
      let ct = response.headers.get("Content-Type");
      if (ct != null && ct.includes("json")) {
        let result = await response.json();
        if (result != null && result.success) {
          $record.remove();
          showToast(ew.language.phrase("DeleteSuccess"), "success");
        } else {
          showToast(getError(result) || ew.language.phrase("DeleteFailed"));
        }
      } else {
        _alert(await response.text());
      }
    };

    // Disable inline buttons
    let _disableInlineButtons = () => $__default.default(".ew-list-options, .ew-list-option-body").find("[data-ew-action='inline'][data-action='add'],[data-ew-action='inline'][data-action='copy'],[data-ew-action='inline'][data-action='edit']").addClass("disabled");

    // Enable inline buttons
    let _enableInlineButtons = () => $__default.default(".ew-list-options, .ew-list-option-body").find("[data-ew-action='inline'][data-action='add'],[data-ew-action='inline'][data-action='copy'],[data-ew-action='inline'][data-action='edit']").removeClass("disabled");

    // Get table
    let _getTable = () => $grid.find(".ew-grid-middle-panel .ew-table")[0];

    // Update row index
    let _updateRowIndexes = tbl => {
      let j = 0;
      Array.from(tbl.rows).forEach(row => row.dataset.rowindex = row.dataset.rowindex === "0" ? "0" : String(j++)); // Update row index if not Inline-Add/Copy row
    };

    // Success handler for inline actions
    let _success = async function (response) {
      if (!(response instanceof Response))
        // Inline actions only
        return;
      let text = await response.clone().text(),
        $data = $__default.default("<div>" + text + "</div>"),
        $row = $data.find("tr[data-rowindex=0], div[data-rowindex=0]" + (key ? ", tr[data-key='" + key + "'], div[data-key='" + key + "']" : "")).filter("[data-rowtype=2], [data-rowtype=3]"),
        // Find Inline-Add first
        inline = $row.data("inline"),
        rowtype = $row.data("rowtype"),
        rowindex = inline && rowtype == 2 /* Inline-Add */ ? 0 : inline && rowtype == 3 /* Inline-Edit */ ? 1 : $row.data("rowindex"),
        $els = $row.add($data.find("script[data-rowindex=" + rowindex + "]")); // Add script for the row
      if (["add", "copy", "edit"].includes(action) && !$els[0])
        // Edit row not found => Error
        return showToast(text);
      if (["add", "copy"].includes(action)) {
        // Inline-Add/Copy
        if (document.querySelector("main.ew-no-record") && $data.find("main:not(.ew-no-record)")[0]) {
          // No existing record, new Inline-Add/Copy row found
          await refresh(Promise.resolve(response));
        } else {
          if ($row.is("TR")) {
            // Table
            let tbl = _getTable();
            if (tbl.matches(".ew-infinite-scroll-table")) $__default.default(tbl.tBodies).last().append($els);else $__default.default(tbl.tBodies).last().prepend($els);
            _updateRowIndexes(tbl); // Update row index
            setupTable(tbl, true);
          } else {
            $grid.find(".ew-multi-column-row").append($els);
          }
          _disableInlineButtons();
        }
        ew.initPage($__default.default.Event({
          type: "load.ew",
          target: $grid[0]
        }));
        forms.get(f.id).tryFocus();
        scrollIntoView($grid.find("[data-rowtype=2]")[0]);
      } else if (action == "edit") {
        // Inline-Edit
        $detached = $record.replaceWith($els); // Replace current row
        $grid.data("detached", $detached); // Save the detached row
        if ($row.is("TR"))
          // Table
          setupTable(_getTable(), true);
        _disableInlineButtons();
        ew.initPage($__default.default.Event({
          type: "load.ew",
          target: $grid[0]
        }));
        forms.get(f.id).tryFocus();
        scrollIntoView($grid.find("[data-rowtype=3]")[0]);
      }
    };

    // Submit success handler for inline actions
    let _submitSuccess = async function (response) {
      if (!(response instanceof Response)) return;
      let ct = response.headers.get("Content-Type");
      if (ct != null && ct.includes("json")) {
        let result = await response.json(),
          error = getError(result);
        if (error) {
          showToast(error);
          return;
        }
      }
      if (response.headers.get("Infinite-Scroll")) {
        // Infinite scroll
        let text = await response.text(),
          $data = $__default.default("<div>" + text + "</div>"),
          $main = $data.find(".main"),
          $row;
        if (["insert", "update"].includes(action)) {
          $row = $data.find("tr[data-rowindex][data-rowtype=" + ew.ROWTYPE_VIEW + "], div[data-rowindex][data-rowtype=" + ew.ROWTYPE_VIEW + "]"); // Row(s) with ROWTYPE_VIEW
          if (!$row[0]) return; // Affected row(s) not found
          if (action == "insert")
            // Inline-Add/Copy
            $grid.find("tr[data-rowtype=" + ew.ROWTYPE_ADD + "], div[data-rowtype=" + ew.ROWTYPE_ADD + "]").replaceWith($row[0]); // Replace add row
          else if (action == "update")
            // Inline-Edit/Update
            $row.get().forEach(row => $grid.find("tr[data-key='" + row.dataset.key + "'], div[data-key='" + row.dataset.key + "']").replaceWith(row)); // Replace row(s) edited/updated
        } else if (["add", "edit", "multiedit"].includes(action)) {
          // Grid-Add/Grid-Edit/Multi-Edit
          $row = $data.find("tr[data-rowindex][data-rowtype=" + ew.ROWTYPE_VIEW + "], div[data-rowindex][data-rowtype=" + ew.ROWTYPE_VIEW + "]"); // Row(s) with ROWTYPE_VIEW
          if (["edit", "multiedit"].includes(action))
            // Grid-Edit/Multi-Edit
            $row.get().forEach(row => $grid.find("tr[data-key='" + row.dataset.key + "'], div[data-rowtype='" + row.dataset.key + "']").replaceWith(row));else if (action == "add")
            // Grid-Add
            $grid.find(".ew-grid-middle-panel .ew-table > tbody:last").append($row); // Append to bottom
        }

        let tbl = _getTable();
        _updateRowIndexes(tbl); // Update row index
        setupTable(tbl, true);
        showMessage({
          target: $data
        }); // Show message, if any
        ew.initPage($__default.default.Event({
          type: "load.ew",
          target: $main[0]
        }));
      } else {
        // Use Ajax actions
        await refresh(Promise.resolve(response));
      }
      let returnUrl = response.headers.get("Return-Url"); // Check if return URL exists
      if (returnUrl && ["inline", "modal"].includes(args.ewAction) && !samePath(parseUrl(returnUrl).pathname, currentUrl.pathname)) $grid.find("[data-ew-action=modal][data-url='" + returnUrl + "'], [data-ew-action=inline][data-url='" + returnUrl + "']").trigger("click");
    };
    if (ewAction == "inline") {
      if (action == "cancel") {
        if (curindex == "0") {
          // Cancel Inline-Add/Copy
          $record.remove();
        } else {
          var _$grid$data;
          // Cancel Inline-Edit
          (_$grid$data = $grid.data("detached")) == null ? void 0 : _$grid$data.replaceAll($record);
        }
        _enableInlineButtons();
      } else if (f && ["insert", "update"].includes(action)) {
        // Post form
        let frm = forms.get(f.id);
        if (await frm.canSubmit(args.evt)) {
          // Validate
          document.body.style.cursor = "wait";
          let body = setSearchParams($f.serialize(), {
              action
            }),
            // Set up action
            p = _fetch(setSearchParams(url, {
              rnd: random()
            }), {
              method: "POST",
              body
            }).catch(_fail).finally(() => {
              frm.enableForm();
              _always();
            });
          if (frm.submitWithFetch) {
            let args = {
                form: f,
                result: p
              },
              evt = $__default.default.Event("aftersubmit", {
                originalEvent: args.evt
              });
            frm.trigger(evt, [args]);
          } else {
            p.then(_submitSuccess).finally(_enableInlineButtons);
          }
        }
      } else if (action == "delete") {
        // Inline-Delete
        if (!key) {
          // No key from the clicked row => Multi-Delete
          $record = $grid.find("input[type=checkbox][name='key_m[]']:checked").closest("tr[data-rowindex], div[data-rowindex]"); // Update $record
          if (!$record[0]) {
            _alert(ew.language.phrase("NoRecordSelected"));
            return false;
          }
        }
        _prompt(ew.language.phrase("DeleteConfirm"), result => {
          if (result) {
            document.body.style.cursor = "wait";
            let method = key ? "GET" : "POST",
              body = deleteSearchParam($f.serialize(), "action"),
              frm = forms.get(f.id),
              p = _fetch(setSearchParams(url, {
                action: "delete",
                rnd: random()
              }), {
                method,
                body
              }).catch(_fail).finally(() => {
                frm.enableForm();
                _always();
              });
            if (frm.submitWithFetch) {
              let args = {
                  form: f,
                  result: p
                },
                evt = $__default.default.Event("aftersubmit", {
                  originalEvent: args.evt
                });
              frm.trigger(evt, [args]);
            } else {
              p.then(_deleted);
            }
          }
        });
      } else {
        document.body.style.cursor = "wait";
        _fetch(setSearchParams(url, {
          rnd: random()
        })).then(_success).catch(_fail).finally(_always);
      }
    } else if (ewAction == "grid" || ewAction == "modal") {
      // Grid-Add/Edit, Modal-Add/Copy/Edit/Update
      if ((ewAction == "grid" && action == "edit" || ewAction == "modal" && ["multiedit", "update"].includes(action)) && !keySelected(f)) {
        // Grid-Edit and Multi-Edit/Update
        _alert(ew.language.phrase("NoRecordSelected"));
        return false;
      }
      ew.modalDialogShow({
        ...args,
        callback: _submitSuccess
      });
    }
    return false;
  }

  // Confirm inline-delete
  function confirmDelete(el) {
    clickDelete(el);
    _prompt(ew.language.phrase("DeleteConfirm"), result => {
      if (result) {
        if (el.dataset.json && el.href)
          // Handle JSON response
          ew.modalDialogShow({
            url: setSearchParams(el.href, {
              action: "delete"
            }),
            json: true,
            callback: true
          });else if (el.href) window.location = sanitizeUrl(setSearchParams(el.href, {
          action: "delete"
        }));else clearDelete(el);
      }
    });
    return false;
  }

  // Check if any key selected // PHP
  function keySelected(f) {
    return !!(f != null && f.querySelector("input[type=checkbox][name='key_m[]']:checked"));
  }

  // Select all keys
  function selectAllKeys(cb) {
    selectAll(cb);
    let tbl = cb == null ? void 0 : cb.closest(".ew-table");
    if (!tbl) return;
    $__default.default(tbl.tBodies).each(function () {
      $__default.default(this.rows).each(function () {
        let $r = $__default.default(this);
        if ($r.is(":not(.ew-template):not(.ew-table-preview-row)")) $r.toggleClass("table-active ew-table-selected-row", cb.checked).triggerHandler("change");
      });
    });
  }

  // Select all related checkboxes in the form
  function selectAll(cb) {
    if (!(cb != null && cb.form)) return;
    $__default.default(cb.form.elements).filter("input[type=checkbox][name^=" + cb.name + "_], [type=checkbox][name=" + cb.name + "]").not(cb).not(":disabled").prop("checked", cb.checked);
  }

  // Update selected checkbox
  function updateSelected(f) {
    return !!(f != null && f.querySelector("input[type=checkbox][name^=u_]:checked,input[type=hidden][name^=u_][value='1']"));
  }

  // Clear selected rows color
  function clearSelected(tbl) {
    let rowIndexes = $__default.default(tbl).find("input[type=checkbox][name='key_m[]']:checked").closest("[data-rowindex]").map((i, r) => r.dataset.rowindex).get();
    $__default.default(tbl == null ? void 0 : tbl.rows).filter((i, r) => r.classList.contains("table-active") && !rowIndexes.includes(r.dataset.rowindex)).removeClass("table-active ew-table-selected-row").triggerHandler("change");
  }

  // Clear all row delete status
  function clearDelete(el) {
    let tbl = el == null ? void 0 : el.closest(".ew-table");
    if (!tbl) return;
    let $tr = $__default.default(el).closest(".ew-table > tbody > tr");
    $tr.siblings("[data-rowindex='" + $tr.data("rowindex") + "']").addBack().removeClass("table-active").triggerHandler("change");
  }

  // Click single delete link
  function clickDelete(el) {
    let tbl = el == null ? void 0 : el.closest(".ew-table");
    if (!tbl) return;
    clearSelected(tbl);
    let $tr = $__default.default(el).closest(".ew-table > tbody > tr");
    $tr.siblings("[data-rowindex='" + $tr.data("rowindex") + "']").addBack().addClass("table-active").triggerHandler("change");
  }

  // Select a row
  function selectKey(e) {
    let chk = e.target,
      tbl = chk == null ? void 0 : chk.closest(".ew-table");
    if (!tbl) return;
    clearSelected(tbl);
    let $tr = $__default.default(chk).closest(".ew-table > tbody > tr");
    $tr.siblings("[data-rowindex='" + $tr.data("rowindex") + "']").addBack().each(function () {
      $__default.default(this).toggleClass("table-active ew-table-selected-row", chk.checked).triggerHandler("change");
    });
    e.stopPropagation();
  }

  /**
   * Setup table
   *
   * @param {number} [index=undefined] Index
   * @param {HTMLTableElement} tbl HTML table element
   * @param {boolean} [force=undefined] Force setup
   * @returns
   */
  function setupTable() {
    let tbl, force;
    if ($__default.default.isNumber(arguments[0])) {
      tbl = arguments[1];
    } else if (arguments[0] instanceof HTMLTableElement) {
      tbl = arguments[0];
      force = arguments[1];
    }
    let $tbl = $__default.default(tbl),
      $rows = $__default.default(tbl.rows);
    if (!tbl || !tbl.rows || !force && tbl.dataset.isset || tbl.tBodies.length == 0) return;
    // Set selected row color
    let click = function (e) {
      let $this = $__default.default(this),
        tbl = this.closest(".ew-table"),
        $target = $__default.default(e.target);
      if (!tbl || $target.hasClass("btn") || $target.hasClass("ew-preview-btn") || $target.is(":input")) return;
      clearSelected(tbl); // Clear all other selected rows
      $this.siblings("[data-rowindex='" + $this.data("rowindex") + "']").addBack().toggleClass("table-active").triggerHandler("change");
    };
    let n = $rows.filter("[data-rowindex=1]").length || $rows.filter("[data-rowindex=0]").length || 1,
      // Alternate color every n rows
      rows = $rows.filter(":not(.ew-template)").each(function () {
        $__default.default(this.cells).removeClass("ew-table-last-row").last().addClass("ew-table-last-col"); // Cell of last column
      }).get();
    if (rows.length >= n) {
      let div = $tbl.parentsUntil(".ew-grid", "." + ew.RESPONSIVE_TABLE_CLASS)[0];
      rows[rows.length - 1].classList.add("border-bottom-0"); // Last row
      rows.splice(n * -1).forEach((row, i) => Array.from(row.cells).filter(cell => cell.rowSpan == i + 1) // Cell of last row
      .forEach(cell => {
        cell.classList.add("ew-table-last-row");
        cell.classList.toggle("ew-table-border-bottom", (div == null ? void 0 : div.clientHeight) > tbl.offsetHeight);
      }));
    }
    let form = tbl.closest("form"),
      attach = form && $__default.default(form.elements).filter("input#action:not([value^=grid])").length > 0;
    $__default.default(tbl.tBodies[tbl.tBodies.length - 1].rows) // Use last TBODY (avoid Opera bug)
    .filter(":not(.ew-template):not(.ew-table-preview-row)").each(function () {
      let $r = $__default.default(this);
      if (attach && !$r.data("isset")) $r.on("click", click).data("isset", true);
    });
    setupGrid(tbl.closest(".ew-grid"), force);
    tbl.dataset.isset = "true";
  }

  // Setup grid
  function setupGrid(grid, force) {
    let $grid = $__default.default(grid);
    if (!grid || !force && grid.dataset.isset) return;
    let rowcnt = $grid.find("table.ew-table > tbody").first().children("tr:not(.ew-table-preview-row, .ew-template)").length;
    if (rowcnt == 0 && !grid.querySelector(".ew-grid-upper-panel, .ew-grid-lower-panel")) $grid.hide();
    if ($grid.find(".ew-grid-middle-panel:visible").hasClass(ew.RESPONSIVE_TABLE_CLASS) && $grid.width() > $__default.default(".content").width()) {
      $grid.addClass("d-flex");
      $grid.closest(".ew-detail-pages").addClass("d-block");
      $grid.closest(".ew-form").addClass("w-100");
    }
    grid.dataset.isset = "true";
  }

  // Add a row to grid
  function addGridRow(el) {
    var _bootstrap$Tooltip$ge2;
    let grid = el == null ? void 0 : el.closest(".ew-grid"),
      $grid = $__default.default(grid),
      $tbl = $grid.find("table.ew-table").last(),
      $p = $tbl.parent("div"),
      $tpl = $tbl.find("tr.ew-template");
    if (!el || !grid || !$tbl[0] || !$tpl[0]) return false;
    let $lastrow = $__default.default($tbl[0].rows).last();
    $tbl.find("td.ew-table-last-row").removeClass("ew-table-last-row");
    let $row = $tpl.clone(true, true).removeClass("ew-template"),
      form = grid.querySelector("div.ew-form[id^=f][id$=grid]") || grid.querySelector("form.ew-form[id^=f][id$=list]") || grid.querySelector("form.ew-form[id^=f][id$=grid]"),
      $form = $__default.default(form),
      suffix = $form.is("div") ? "_" + form.id : "",
      $keycnt = $form.find("#" + ew.FORM_KEY_COUNT_NAME + suffix),
      keycnt = parseInt($keycnt.val(), 10) + 1,
      name = ew.FORM_ROW_ACTION_NAME.replace(/^k_/, "k" + keycnt + "_") + suffix,
      // name="k<n>_action"
      $els = $tpl.find("script:contains('$rowindex$')").add("tr.ew-template + script[data-rowindex='$rowindex$']"); // Get scripts with rowindex
    $row.attr({
      "id": "r" + keycnt + $row.attr("id").substring(2),
      "data-rowindex": keycnt
    });
    $row.children("td").each(function () {
      $__default.default(this).find("*").each(function () {
        $__default.default.each(this.attributes, function (i, attr) {
          attr.value = attr.value.replace(/\$rowindex\$/g, keycnt); // Replace row index
        });
      });
    });

    let $btn = $row.find(".ew-icon").closest("a, button");
    (_bootstrap$Tooltip$ge2 = bootstrap.Tooltip.getInstance($btn[0])) == null ? void 0 : _bootstrap$Tooltip$ge2.dispose();
    $btn.tooltip({
      ...ew.tooltipOptions,
      container: "body",
      trigger: "hover"
    });
    $keycnt.val(keycnt).after($__default.default("<input>").attr({
      type: "hidden",
      id: name,
      name: name,
      value: "insert"
    }));
    $lastrow.after($row);
    $els.get().forEach(el => addScript(el.text.replace(/\$rowindex\$/g, keycnt)));
    let frm = $form.data("form");
    frm == null ? void 0 : frm.initEditors();
    frm == null ? void 0 : frm.initUpload();
    setupTable($tbl[0], true);
    $p.scrollTop($p[0].scrollHeight);
    $tbl.trigger("rowadded", [keycnt]);
    return false;
  }

  // Delete a row from grid
  function deleteGridRow(el, infix) {
    var _bootstrap$Tooltip$ge3;
    (_bootstrap$Tooltip$ge3 = bootstrap.Tooltip.getInstance(el)) == null ? void 0 : _bootstrap$Tooltip$ge3.dispose();
    let grid = el.closest(".ew-grid, .ew-multi-column-grid"),
      row = el.closest("tr, div[data-rowindex]"),
      tbl = row == null ? void 0 : row.closest(".ew-table"),
      rowidx = parseInt(row == null ? void 0 : row.dataset.rowindex, 10),
      form = (grid == null ? void 0 : grid.querySelector("div.ew-form[id^=f][id$=grid]")) || (grid == null ? void 0 : grid.querySelector("form.ew-form[id^=f][id$=list], form.ew-form[id^=f][id$=grid]")),
      $form = $__default.default(form),
      frm = $form.data("form");
    if (!el || !grid || !row || !tbl || !form || !frm) return false;
    let suffix = $form.is("div") ? "_" + form.id : "",
      keycnt = "#" + ew.FORM_KEY_COUNT_NAME + suffix;
    let _delete = function () {
      row.remove();
      if (grid.classList.contains(".ew-grid")) setupTable(tbl, true);
      if (rowidx > 0) {
        let $keyact = $form.find("#k" + rowidx + "_action" + suffix);
        if ($keyact[0]) {
          $keyact.val($keyact.val() == "insert" ? "insertdelete" : "delete");
        } else {
          $form.find(keycnt).after($__default.default("<input>").attr({
            type: "hidden",
            id: "k" + rowidx + "_action" + suffix,
            name: "k" + rowidx + "_action" + suffix,
            value: "delete"
          }));
        }
      }
      $__default.default(tbl).trigger("rowdeleted", [rowidx]);
    };
    if (isFunction$2(frm.emptyRow) && frm.emptyRow(infix)) {
      // Empty row
      _delete();
    } else {
      // Confirm
      _prompt(ew.language.phrase("DeleteConfirm"), result => {
        if (result) _delete();
      });
    }
    return false;
  }

  // HTML encode text
  function htmlEncode(text) {
    return String(text).replace(/&/g, '&amp;').replace(/\"/g, '&quot;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
  }

  // HTML decode text
  function htmlDecode(text) {
    return String(text).replace(/&amp;/g, '&').replace(/&quot;/g, '"').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
  }

  // Get form element(s) as single element or array of radio/checkbox
  function getElements(el, root) {
    root = !$__default.default.isString(root) ? root : /^#/.test(root) ? root : "#" + root;
    let $root = root ? $__default.default(root) : $document$1,
      selectors = [],
      getFieldSelector = field => "[data-field='" + field + "']:not(.ew-custom-option):not([name^=y_]):not([data-old])";
    if (el instanceof HTMLElement && el.dataset.table && el.dataset.field) {
      // HTML element (e.g. radio/checkbox)
      if (isBooleanCheckbox(el)) return el;
      selectors = ["[data-table='" + el.dataset.table + "']" + getFieldSelector(el.dataset.field)];
    } else if ($__default.default.isString(el)) {
      let [s1, s2] = el.split(" "); // Check if "<table> <name>"
      selectors = s2 ? ["[data-table='" + s1 + "']" + getFieldSelector(getId(s2))] // "<table> <name>", remove []
      : ["[name='" + el + "']:not(.ew-custom-option):not([data-old])", getFieldSelector(getId(el))]; // "<name>" only
    }

    let selector = ["input", "select", "textarea", "button", "selection-list"].flatMap(tag => selectors.map(sel => tag + sel)).join(),
      $els = $root.find(selector);
    if ($root.is("form.ew-form") && $root.find("div.ew-form")[0])
      // Master/Detail form
      $els = $els.not("div.ew-form *"); // Remove elements of detail tables
    if ($els.length == 1 && ($els.is(":not([type=checkbox]):not([type=radio])") || isBooleanCheckbox($els[0])) || $els.length == 2 && $els.eq(0).is("selection-list") && $els.eq(1).is("input[type=hidden]"))
      // Polyfill for the ElementInternals
      return $els[0];
    return $els[0] ? $els.get() : null;
  }

  // Get first element by id/name (not necessarily form element)
  function getElement(name, root) {
    root = !$__default.default.isString(root) ? root : /^#/.test(root) ? root : "#" + root;
    let selector = "#" + name.replace(/([\$\[\]])/g, "\\$1") + ",[name='" + name + "']";
    return root ? $__default.default(root).find(selector)[0] : $__default.default(selector).first()[0];
  }

  // Get ancestor by function
  function getAncestorBy(node, fn) {
    while (node = node.parentNode) {
      var _node;
      if (((_node = node) == null ? void 0 : _node.nodeType) == 1 && (!fn || fn(node))) return node;
    }
    return null;
  }

  // Check if an element is hidden
  function isHidden(el) {
    var $el = $__default.default(el);
    return $el.css("display") == "none" && !$el.is("selection-list") && !$el.closest(".dropdown-menu")[0] && !isModalLookup(el) && !isAutoSuggest(el) && !isHiddenTextArea(el) || getAncestorBy(el, node => node.style.display == "none" && !node.classList.contains("tab-pane") && !node.classList.contains("collapse")) != null;
  }

  // Check if same text
  function sameText(o1, o2) {
    return String(o1).toLowerCase() == String(o2).toLowerCase();
  }

  // Check if same string
  function sameString(o1, o2) {
    return String(o1) == String(o2);
  }

  // Convert to header case
  function headerCase(str) {
    return !str ? "" : String(str).replace(/^[^A-Za-z0-9]*|[^A-Za-z0-9]*$/g, "").replace(/([a-z])([A-Z])/g, (m, a, b) => a + "_" + b.toLowerCase()).replace(/[^A-Za-z0-9]+|_+/g, "-").toLowerCase().replace(/(-?)(\w+)(-?)/g, (m, a, b, c) => a + b.charAt(0).toUpperCase() + b.slice(1) + c);
  }

  // Check if same path
  function samePath(p1, p2) {
    p1 = p1 instanceof URL ? p1.pathname : parseUrl(p1).pathname;
    p2 = p2 instanceof URL ? p2.pathname : parseUrl(p2).pathname;
    if (p1 != p2 && p1.startsWith(ew.PATH_BASE) && p2.startsWith(ew.PATH_BASE)) {
      p1 = p1.substring(ew.PATH_BASE.length);
      p2 = p2.substring(ew.PATH_BASE.length);
      let ar1 = p1.split("/"),
        ar2 = p2.split("/");
      if (ar1[0] == ar2[0] && ar1[0].match(/(list|view|edit)$/i) ||
      // *list == *list
      ar1[0] == ar2[0] && ar1[1] == ar2[1] && [ew.LIST_ACTION, ew.VIEW_ACTION, ew.EDIT_ACTION].includes(ar1[1]))
        // xxx/list == xxx/list
        return true;
    }
    return p1 == p2;
  }

  // Get element value as array for select-multiple field or else as string
  function getValue(el, form) {
    let obj = el;
    if ($__default.default.isString(el)) obj = el.split(" ").length == 2 // Parent field in master table
    ? getElements(el) : getElements(el, form);else if (["checkbox", "radio"].includes(el.type))
      // Single radio/checkbox
      obj = getElements(el); // Find radio buttons and checkboxes with the same name
    if (!obj) return undefined;
    if (obj.options) {
      // Selection list
      return obj.list ? obj.type == "select-multiple" ? obj.values : obj.value : $__default.default(obj).val();
    } else if (Array.isArray(obj)) {
      // Radio/Checkbox list or element not found
      return obj.length ? $__default.default(obj).filter(":checked").map((i, el) => el.value).get() : undefined;
    } else if (isBooleanCheckbox(obj)) {
      // Single checkbox
      return obj.checked ? obj.value : undefined;
    } else if (ew.isHiddenTextArea(obj)) {
      $__default.default(obj).data("editor").save();
      return obj.value;
    } else {
      // text/hidden
      return obj.value;
    }
  }

  // Get existing selected values as an array
  function getOptionValues(el, form) {
    let obj = el;
    if ($__default.default.isString(el)) obj = el.split(" ").length == 2 // Parent field in master table
    ? getElements(el) : getElements(el, form);else if (["checkbox", "radio"].includes(el.type))
      // Single radio/checkbox
      obj = getElements(el); // Find radio buttons and checkboxes with the same name
    if (!obj) return [];
    if (obj.options) {
      // Selection list
      if (obj.list) {
        return obj.values;
      } else if (isSelect2(obj)) {
        let val = $__default.default(obj).val();
        return Array.isArray(val) ? val : $__default.default.isValue(val) ? [val] : [];
      } else {
        return Array.from(obj.options).filter(option => option.selected && option.value !== "").map(option => option.value);
      }
    } else if (Array.isArray(obj) || isBooleanCheckbox(obj)) {
      // Radio/Checkbox list, or single checkbox, or element not found ([])
      return $__default.default(obj).filter(":checked").map(function () {
        return this.value;
      }).get();
    } else if (ew.isHiddenTextArea(obj)) {
      $__default.default(obj).data("editor").save();
      return [obj.value];
    } else {
      // text/hidden
      return [obj.value];
    }
  }

  // Get existing selected values as string
  function getOptionValue(el, form) {
    return getOptionValues(el, form).join(ew.MULTIPLE_OPTION_SEPARATOR);
  }

  // Get existing text of selected values as an array
  function getOptionTexts(el, form) {
    let obj = el;
    if ($__default.default.isString(el)) obj = el.split(" ").length == 2 // Parent field in master table
    ? getElements(el) : getElements(el, form);else if (["checkbox", "radio"].includes(el.type))
      // Single radio/checkbox
      obj = getElements(el); // Find radio buttons and checkboxes with the same name
    if (!obj) return [];
    if (isAutoSuggest(obj)) {
      // AutoSuggest (before obj.options)
      return [obj.input.value];
    } else if (obj.options) {
      // Selection list
      return isSelect2(obj) ? $__default.default(obj).find(":selected").map((i, el) => el.innerHTML).get() : Array.from(obj.options).filter(option => option.selected && option.value !== "").map(option => option.text);
    } else if (Array.isArray(obj) || isBooleanCheckbox(obj)) {
      // Radio/Checkbox list, or single checkbox, or element not found ([])
      return $__default.default(obj).filter(":checked").map(function () {
        return $__default.default(this).parent().text();
      }).get();
    } else if (ew.isHiddenTextArea(obj)) {
      $__default.default(obj).data("editor").save();
      return [obj.value];
    } else {
      return [obj.value];
    }
  }

  // Clear existing options
  function clearOptions(obj) {
    if (obj.options) {
      // Selection list
      var lo = obj.type == "select-multiple" ||
      // multiple
      obj.hasAttribute("data-dropdown") ||
      // dropdown
      obj.length > 0 && obj.options[0].value != "" // non-empty first element
      ? 0 : 1;
      if (obj.list) {
        obj.removeAll();
        obj.render();
      } else {
        for (var i = obj.length - 1; i >= lo; i--) obj.remove(i);
      }
      if (isAutoSuggest(obj)) {
        obj.input.value = "";
        obj.value = "";
      }
    }
  }

  /**
   * Get the name or id of an element
   *
   * @param {HTMLElement} el - HTML element
   * @param {boolean} [remove=true] - Remove square brackets
   * @returns
   */
  function getId(el, remove) {
    var id = $__default.default.isString(el) ? el : $__default.default(el).attr("name") || $__default.default(el).attr("id"); // Use name first (id may have suffix)
    return remove !== false ? id.replace(/\[\]$/, "") : id;
  }

  // Get display value separator
  function valueSeparator(index, obj) {
    var sep = $__default.default(obj).data("value-separator");
    return Array.isArray(sep) ? sep[index - 1] : sep || ", ";
  }

  /**
   * Get display value
   *
   * @param {Object} opt - Option being displayed
   * @param {HTMLElment} obj - HTML element
   * @returns {string} Display value
   */
  function displayValue(opt, obj) {
    return [opt.df, opt.df2, opt.df3, opt.df4].reduce((text, value, i) => {
      let sep = i > 0 ? valueSeparator(i - 1, obj) : "";
      return !$__default.default.isUndefined(sep) && $__default.default.isValue(value) && value != "" ? text + sep + value : text;
    }, "");
  }

  /**
   * Get HTML for a single option
   *
   * @param {*} val - Value of the option
   * @returns {string} HTML
   */
  function optionHtml(val) {
    return ew.OPTION_HTML_TEMPLATE.replace(/\{value\}/g, val);
  }

  /**
   * Get HTML for diplaying all options
   *
   * @param {string[]} options - Array of all options (HTML)
   * @param {number} max - Maximum number of options to show
   * @returns {string} HTML
   */
  function optionsHtml(options, max) {
    if (options.length > (max || ew.MAX_OPTION_COUNT))
      // More than max option count
      return ew.language.phrase("CountSelected").replace("%s", options.length);else if (options.length)
      // Some options
      return options.reduce((previous, current) => previous + optionHtml(current), "");
    return ew.language.phrase("PleaseSelect"); // No options
  }

  /**
   * Create new option
   *
   * @param {(HTMLElement|array)} obj - Selection list
   * @param {Object} data - Object for the new option
   * @param {form} f - form object of obj
   * @param {bool} selected - New option is selected
   * @returns
   */
  function newOption(obj, data, f, selected) {
    let frm = forms.get(f.id),
      id = getId(obj),
      list = frm.getList(obj),
      group = data.gf,
      value = data.lf,
      template = frm.getOptionTemplate(list),
      text = template != null && template.render && !isAutoSuggest(obj) ? template.render(data, ew.jsRenderHelpers) : displayValue(data, obj) || value;
    if (obj.options) {
      // Selection list
      let option;
      if (obj.list) {
        option = new ew.SelectionListOption(text, value, !!selected);
      } else {
        option = document.createElement("option");
        option.value = value;
        option.innerHTML = text;
        option.selected = !!selected;
      }
      let args = {
        "name": id,
        "form": f.$element,
        data,
        group,
        option
      };
      $document$1.trigger("newoption", [args]); // Fire "newoption" event for selection list
      if (obj.list) {
        obj.addOption(args.option);
      } else {
        if (args.group && !isSelect2(obj)) {
          var _obj$querySelector;
          let optGroup = Array.from((_obj$querySelector = obj.querySelector(":scope > optgroup")) != null ? _obj$querySelector : []).find(g => g.label == args.group);
          if (!optGroup) {
            optGroup = document.createElement("optgroup");
            optGroup.label = args.group;
            obj.add(optGroup);
          }
          optGroup.appendChild(args.option);
        } else {
          obj.add(args.option);
        }
      }
      return args.option.text;
    }
    return text;
  }

  // Select combobox option
  function selectOption(obj, values, change) {
    var _obj$dataset, _forms$get$getList, _forms$get$getList$pa;
    if (change === void 0) {
      change = true;
    }
    if (!obj || !values) return;
    let $obj = $__default.default(obj);
    values = Array.isArray(values) ? values : [values];
    if (obj.options) {
      // Selection list
      if (obj.list) {
        obj.value = values;
      } else {
        var _obj$options$;
        $obj.val(values);
        if (change && isSelect2(obj)) $obj.triggerHandler("change");
        if (obj.type == "select-one" && obj.selectedIndex == -1 && !((_obj$options$ = obj.options[0]) != null && _obj$options$.value)) obj.selectedIndex = 0; // Make sure an option is selected
      }

      if (isAutoSuggest(obj) && values.length == 1) {
        let opts = obj.options || [];
        for (let opt of opts) {
          if (opt.value == values[0]) {
            obj.value = opt.value;
            obj.input.value = opt.text;
            break;
          }
        }
      }
    } else if (isBooleanCheckbox(obj)) {
      obj.checked = convertToBool(values.join(ew.MULTIPLE_OPTION_SEPARATOR));
    } else if (obj.type && obj.type != "file") {
      $obj.val(values.join(ew.MULTIPLE_OPTION_SEPARATOR));
      if (isHiddenTextArea($obj)) $obj.data("editor").set();
    }
    // Auto-select if only one option
    if (((_obj$dataset = obj.dataset) == null ? void 0 : _obj$dataset.autoselect) === "false")
      // data-autoselect="false"
      return;
    let form = getForm(obj),
      autoSelect = form && !(form.id.endsWith("search") || form.id.endsWith("srch") || ((_forms$get$getList = forms.get(form.id).getList(obj)) == null ? void 0 : (_forms$get$getList$pa = _forms$get$getList.parentFields) == null ? void 0 : _forms$get$getList$pa.length) === 0); // Not search forms and has parent fields
    if (autoSelect && obj.options) {
      // Selection List
      if (!obj.list && obj.type == "select-one" && obj.options.length == 2 && !obj.options[1].selected) {
        obj.options[1].selected = true;
        !change || $obj.trigger("change");
      } else if (obj.options.length == 1 && !obj.options[0].selected) {
        obj.options[0].selected = true;
        !change || $obj.trigger("change");
      }
      if (isAutoSuggest(obj)) {
        let opts = obj.options || [];
        if (opts.length == 1) {
          obj.value = opts[0].value;
          obj.input.value = opts[0].text;
        }
      }
    }
  }

  // Fetch API
  function _fetch(url, init) {
    var _init, _init$headers, _init$headers$get;
    (_init = init) != null ? _init : init = {};
    init.headers = new Headers(init.headers || {});
    let isApi = url.startsWith(getApiUrl()); // Is API request
    if (isApi && ew.API_JWT_TOKEN && !ew.IS_WINDOWS_AUTHENTICATION)
      // Do NOT set JWT authorization header if Windows Authentication
      init.headers.set(ew.API_JWT_AUTHORIZATION_HEADER, "Bearer " + ew.API_JWT_TOKEN);
    if (isApi || (_init$headers = init.headers) != null && (_init$headers$get = _init$headers.get("Content-Type")) != null && _init$headers$get.includes("application/json")) {
      // Add token in headers if sending to API or posting JSON data // PHP
      init.headers.set(headerCase(ew.TOKEN_NAME_KEY), ew.TOKEN_NAME); // Send token name in header // PHP
      init.headers.set(headerCase(ew.ANTIFORGERY_TOKEN_KEY), ew.ANTIFORGERY_TOKEN); // Send antiforgery token in header // PHP
    }

    if (!init.method || sameText(init.method, "GET")) {
      // GET
      url = setSearchParams(url, init.body);
      delete init.body;
    } else {
      // POST
      init.body = mergeSearchParams(init.body, {
        [ew.TOKEN_NAME_KEY]: ew.TOKEN_NAME,
        [ew.ANTIFORGERY_TOKEN_KEY]: ew.ANTIFORGERY_TOKEN
      }); // Add token name and antiforgery token // PHP
    }

    let args = {
      url,
      init
    };
    $document$1.trigger("fetch", [args]);
    return fetch(args.url, args.init);
  }

  // Ajax send
  $document$1.ajaxSend(function (event, jqxhr, settings) {
    let url = settings.url,
      isApi = url.startsWith(getApiUrl()),
      // Is API request
      allowed = isApi || url.startsWith(ew.PATH_BASE) || url.startsWith(currentPage());
    if (!allowed && url.match(/^http/i)) {
      let objUrl = new URL(url);
      allowed = objUrl.hostname == currentUrl.hostname; // Same host name
    }

    if (allowed) {
      if (isApi && ew.API_JWT_TOKEN && !ew.IS_WINDOWS_AUTHENTICATION)
        // Do NOT set JWT authorization header if Windows Authentication
        jqxhr.setRequestHeader(ew.API_JWT_AUTHORIZATION_HEADER, "Bearer " + ew.API_JWT_TOKEN); // Note: settings.headers won't work
      if (isApi || $__default.default.isString(settings.contentType) && settings.contentType.includes("application/json")) {
        // Add token and antiforgery token in headers if sending to API or posting JSON data // PHP
        jqxhr.setRequestHeader(headerCase(ew.TOKEN_NAME_KEY), ew.TOKEN_NAME);
        jqxhr.setRequestHeader(headerCase(ew.ANTIFORGERY_TOKEN_KEY), ew.ANTIFORGERY_TOKEN);
      } else if (sameText(settings.type, "POST")) {
        // POST
        let params = mergeSearchParams(new URLSearchParams(settings.data), {
          [ew.TOKEN_NAME_KEY]: ew.TOKEN_NAME,
          [ew.ANTIFORGERY_TOKEN_KEY]: ew.ANTIFORGERY_TOKEN
        }); // Add token name and antiforgery token // PHP
        settings.data = params.toString();
      }
    }
  });

  // Ajax start
  $document$1.ajaxStart(function () {
    $document$1.data("_ajax", true);
    $__default.default("form.ew-form").addClass("ew-wait").each(function () {
      var frm = forms.get(this.id);
      if (frm) {
        if (!frm.multiPage || !frm.multiPage.lastPageSubmit) frm.disableForm();
      }
    });
  });

  // Ajax stop (internal)
  function _ajaxStop() {
    $__default.default("form.ew-form.ew-wait").removeClass("ew-wait").each(function () {
      var frm = forms.get(this.id);
      if (frm) {
        if (!frm.multiPage || !frm.multiPage.lastPageSubmit) {
          frm.enableForm();
        }
      }
    });
    $document$1.data("_ajax", false);
  }

  // Ajax stop/error
  $document$1.ajaxStop(_ajaxStop).ajaxError(_ajaxStop);

  // Execute JavaScript in HTML loaded by Ajax
  function executeScript(html, id, modal) {
    let pattern = modal ? /<(head)[^>]*>[\s\S]*?<\/\1\s*>/ig : /<(head|template)[^>]*>[\s\S]*?<\/\1\s*>/ig,
      // If modal, scripts in template tags not executed by browser, execute manually
      matches = html.replaceAll(pattern, "").matchAll(/<script[^>]*>[\s\S]*?<\/script\s*>/ig); // Do not execute scripts in template tags, use non-greedy [\s\S]*?
    Array.from(document.createRange().createContextualFragment(Array.from(matches).map(m => m[0]).filter(s => !s.includes("$rowindex$")).join("")).querySelectorAll("script:not([type]), script[type='text/javascript']")).sort((s1, s2) => s2.classList.contains("ew-apply-template") ? 1 : s1.classList.contains("ew-apply-template") ? -1 : 0) // Execute custom template first
    .forEach((s, i) => addScript(s, "scr_" + id + "_" + i));
  }

  // Strip JavaScript in HTML loaded by Ajax
  function stripScript(html) {
    let matches = html.replaceAll(/<(head|template)[^>]*>[\s\S]*?<\/\1\s*>/ig, "").matchAll(/<script([^>]*)>([\s\S]*?)<\/script\s*>/ig); // Do not strip scripts in template tags, use non-greedy [\s\S]*?
    return Array.from(matches).filter(m => document.createRange().createContextualFragment(m[0]).querySelector("script:not([type]), script[type='text/javascript']")).reduce((html, m) => html.replace(m[0], ""), html);
  }

  // Add SCRIPT tag
  function addScript(text, id) {
    let scr = text instanceof HTMLScriptElement ? text : document.createElement("SCRIPT");
    if ($__default.default.isString(text)) scr.text = text;
    if (id) scr.id = id;
    return document.body.appendChild(scr); // Do not use jQuery so it can be removed
  }

  // Remove JavaScript added by Ajax
  function removeScript(id) {
    if (id) $__default.default("script[id^='scr_" + id + "_']").remove();
  }

  /**
   * Clean HTML loaded by Ajax for modal dialog
   *
   * @param {string} html - HTML string
   * @param {bool} keepScripts - Keep script tags or not
   * @returns HTML string
   */
  function getContent(html, keepScripts) {
    var _m$;
    html = html.trim().replace(/<head>[\s\S]*<\/head>/, "");
    let body = keepScripts ? html : stripScript(html),
      m = body.match(/<body[\s\S]*>[\s\S]*<\/body>/i);
    body = (_m$ = m == null ? void 0 : m[0]) != null ? _m$ : body;
    let $content = $__default.default(body).find("section.content");
    return $content[0] ? $content.html() : body;
  }

  // Get all options of Selection list or Radio/Checkbox list as array
  function getOptions(obj) {
    return obj != null && obj.options ? Array.prototype.map.call(obj.options, opt => [opt.value, opt.text]) : [];
  }

  /**
   * Show dialog for enabling two factor authentication
   */
  function enable2FA() {
    document.body.style.cursor = "wait";
    if (sameText(ew.TWO_FACTOR_AUTHENTICATION_TYPE, "google")) {
      // Show QR Code and Verify
      let url = getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_SHOW]);
      $__default.default.get(url, result => {
        var _result$error4;
        if (result != null && result.url) {
          _prompt({
            imageUrl: result.url,
            html: ew.language.phrase("Scan2FAQrCode"),
            input: "text",
            confirmButtonText: ew.language.phrase("Verify"),
            showLoaderOnConfirm: true,
            allowEscapeKey: false,
            allowOutsideClick: () => !Swal.isLoading(),
            willOpen: () => {
              Swal.showLoading(Swal.getConfirmButton());
              Swal.disableInput();
              Swal.getImage().onload = () => {
                Swal.enableInput();
                Swal.hideLoading();
                Swal.getInput().focus();
              };
            },
            preConfirm: value => {
              return $__default.default.get(getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_VERIFY, value])).then(result => {
                if ((result == null ? void 0 : result.success) !== true) Swal.showValidationMessage(ew.language.phrase("2FAVerificationFailed"));
                return result;
              }).catch(error => Swal.showValidationMessage(error));
            }
          }, result => {
            var _result$error3;
            if (result != null && (_result$error3 = result.error) != null && _result$error3.description) {
              showToast(result.error.description);
            } else if (result != null && result.success) {
              showToast(ew.language.phrase("2FAEnabled"), "success");
              $__default.default("#enable-2fa").addClass("d-none");
              $__default.default("#disable-2fa, #backup-codes").removeClass("d-none");
            }
          }).catch(err => showToast(err == null ? void 0 : err.message));
        } else if (result != null && (_result$error4 = result.error) != null && _result$error4.description) {
          _alert(result.error.description);
        }
      }).fail((jqXHR, textStatus, errorThrown) => showToast(errorThrown)).always(() => document.body.style.cursor = "default");
    } else {
      // Send OTP and Verify
      let url = getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_SHOW]);
      let html = sameText(ew.TWO_FACTOR_AUTHENTICATION_TYPE, "email") ? ew.language.phrase("EnterOTPEmailAddress") : ew.language.phrase("EnterOTPMobileNumber");
      $__default.default.get(url, result => {
        var _result$error7;
        if (result.success) {
          _prompt({
            html: html,
            input: "text",
            inputValue: result.account,
            confirmButtonText: ew.language.phrase("SendOTP"),
            showLoaderOnConfirm: true,
            allowEscapeKey: false,
            allowOutsideClick: () => !Swal.isLoading(),
            preConfirm: value => {
              return $__default.default.get(getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_SEND_OTP, value])).then(result => {
                if ((result == null ? void 0 : result.success) !== true) Swal.showValidationMessage(ew.language.phrase("SendOTPFailed"));
                return result;
              }).catch(error => Swal.showValidationMessage(error));
            }
          }, result => {
            var _result$error5;
            if (result != null && (_result$error5 = result.error) != null && _result$error5.description) {
              showToast(result.error.description);
            } else if (result != null && result.success) {
              // Verify OTP
              _prompt({
                html: ew.language.phrase("EnterSecurityCode"),
                input: "text",
                confirmButtonText: ew.language.phrase("Verify"),
                showLoaderOnConfirm: true,
                allowEscapeKey: false,
                allowOutsideClick: () => !Swal.isLoading(),
                preConfirm: value => {
                  return $__default.default.get(getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_VERIFY, value])).then(result => {
                    if ((result == null ? void 0 : result.success) !== true) Swal.showValidationMessage(ew.language.phrase("2FAVerificationFailed"));
                    return result;
                  }).catch(error => Swal.showValidationMessage(error));
                }
              }, result => {
                var _result$error6;
                if (result != null && (_result$error6 = result.error) != null && _result$error6.description) {
                  showToast(result.error.description);
                } else if (result != null && result.success) {
                  showToast(ew.language.phrase("2FAEnabled"), "success");
                  $__default.default("#enable-2fa").addClass("d-none");
                  $__default.default("#disable-2fa, #backup-codes").removeClass("d-none");
                }
              }).catch(err => showToast(err == null ? void 0 : err.message));
            }
          }).catch(err => showToast(err == null ? void 0 : err.message));
        } else if (result != null && (_result$error7 = result.error) != null && _result$error7.description) {
          _alert(result.error.description);
        }
      }).fail((jqXHR, textStatus, errorThrown) => showToast(errorThrown)).always(() => document.body.style.cursor = "default");
    }
    return false;
  }

  /**
   * Show dialog for disabling two factor authentication
   */
  function disable2FA() {
    _prompt({
      html: ew.language.phrase("Disable2FAConfirm"),
      preConfirm: value => {
        if (value) return $__default.default.get(getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_RESET])).then(result => {
          var _result$error8;
          if (result != null && (_result$error8 = result.error) != null && _result$error8.description) showToast(result.error.description);else if ((result == null ? void 0 : result.success) !== true) showToast(ew.language.phrase("2FAResetFailed"));
          return result;
        }).fail((jqXHR, textStatus, errorThrown) => showToast(errorThrown));
      }
    }, result => {
      var _result$error9;
      if (result != null && (_result$error9 = result.error) != null && _result$error9.description) {
        showToast(result.error.description);
      } else if (result != null && result.success) {
        showToast(ew.language.phrase("2FADisabled"), "success");
        $__default.default("#enable-2fa").removeClass("d-none");
        $__default.default("#disable-2fa, #backup-codes").addClass("d-none");
      }
    }).catch(err => showToast(err == null ? void 0 : err.message));
  }

  /**
   * Show dialog for sending one time password
   */
  function sendOTP(account) {
    _prompt({
      html: ew.language.phrase("SendOTPConfirm"),
      preConfirm: value => {
        let url = account ? getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_SEND_OTP, account]) : getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_SEND_OTP]);
        if (value) return $__default.default.get(url).then(result => {
          var _result$error10;
          if (result != null && (_result$error10 = result.error) != null && _result$error10.description) showToast(result.error.description);else if ((result == null ? void 0 : result.success) !== true) showToast(ew.language.phrase("SendOTPFailed"));
          return result;
        }).fail((jqXHR, textStatus, errorThrown) => showToast(errorThrown));
      }
    }, result => {
      var _result$error11;
      if (result != null && (_result$error11 = result.error) != null && _result$error11.description) {
        showToast(result.error.description);
      } else if (result != null && result.success) {
        showToast(ew.language.phrase("SendOTPSuccess"), "success");
      }
    }).catch(err => showToast(err == null ? void 0 : err.message));
  }

  /**
   * Show backup codes for two factor authentication
   */
  function showBackupCodes() {
    let html = "<p>" + ew.language.phrase("BackupCodesMsg") + "</p>";
    return _alert({
      title: ew.language.phrase("BackupCodes"),
      html: html,
      showDenyButton: true,
      showLoaderOnDeny: true,
      showCancelButton: true,
      confirmButtonText: ew.language.phrase("CopyToClipboard"),
      denyButtonText: ew.language.phrase("GetNewCodes"),
      customClass: {
        denyButton: "btn btn-primary ew-swal2-deny-button"
      },
      willOpen: () => {
        Swal.showLoading();
        Swal.disableButtons();
        $__default.default.get(getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_BACKUP_CODES])).then(result => {
          if (result.success && Array.isArray(result.codes)) {
            Swal.update({
              html: html + "<textarea class=\"form-control ew-backup-codes\" readonly>" + result.codes.join("\n") + "</textarea>"
            });
            Swal.enableButtons();
            Swal.getConfirmButton().focus();
          }
        }).fail((jqXHR, textStatus, errorThrown) => showToast(errorThrown)).always(() => Swal.hideLoading());
      },
      preConfirm: () => {
        let codes = copyToClipboard(Swal.getHtmlContainer().querySelector("textarea"));
        if (codes) showToast(ew.language.phrase("CopiedToClipboard"), "success");
        return false; // Keep the alert open
      },

      preDeny: async () => {
        Swal.showLoading(Swal.getDenyButton());
        await $__default.default.get(getApiUrl([ew.API_2FA_ACTION, ew.API_2FA_NEW_BACKUP_CODES])).then(result => {
          if (result.success && Array.isArray(result.codes)) {
            Swal.update({
              html: html + "<textarea class=\"form-control ew-backup-codes\" readonly>" + result.codes.join("\n") + "</textarea>"
            });
            Swal.getConfirmButton().focus();
          }
        }).fail((jqXHR, textStatus, errorThrown) => showToast(errorThrown)).always(() => Swal.hideLoading());
        return false; // Keep the alert open
      }
    });
  }

  /**
   * Show Add Option dialog
   *
   * @param {Object} args - Arguments
   * @param {MouseEvent} args.evt - Event
   * @param {HTMLElement} args.lnk - Add option anchor element
   * @param {string} args.el - Form element name
   * @param {string} args.url - URL of the Add form
   * @returns
   */
  function addOptionDialogShow(args) {
    var _args$evt2;
    args.lnk = args.lnk || ((_args$evt2 = args.evt) == null ? void 0 : _args$evt2.currentTarget);

    // Hide dialog
    var _hide = function () {
      removeScript($dlg.data("args").el);
      var frm = $dlg.removeData("args").find(".modal-body form").data("form");
      if (frm) frm.destroyEditor();
      $dlg.find(".modal-body").html("");
      $dlg.find(".modal-footer .btn-primary").off();
      $dlg.data("showing", false);
    };
    var $dlg = ew.addOptionDialog || $__default.default("#ew-add-opt-dialog").on("hidden.bs.modal", _hide);
    if (!$dlg[0]) {
      _alert("DIV #ew-add-opt-dialog not found.");
      return;
    }
    if ($dlg.data("showing")) return;
    $dlg.data("showing", true);

    // Submission success
    var _submitSuccess = function (data) {
      var _results;
      var results = data,
        args = $dlg.data("args"),
        frm = forms.get(args.lnk),
        // form object
        objName = $dlg.find(".modal-body form input[name='" + ew.API_OBJECT_NAME + "']").val(),
        // Get object name from form
        el = args.el,
        // HTML element name
        re = /^x(\d+)_/,
        m = el.match(re),
        // Check row index
        prefix = m ? m[0] : "x_",
        index = m ? m[1] : -1,
        name = el.replace(re, "x_"),
        list = frm.getList(el);
      if ($__default.default.isString(data)) results = parseJson(data);
      if ((_results = results) != null && _results.success && results[objName]) {
        // Success
        $dlg.modal("hide");
        var result = results[objName],
          form = frm.$element[0],
          // HTML form or DIV
          obj = getElements(el, form);
        if (obj) {
          var lf = list.linkField,
            dfs = list.displayFields.slice(),
            // Clone
            ffs = list.filterFields.slice(),
            // Clone
            pfs = list.parentFields.slice(); // Clone
          pfs.forEach((pf, i) => {
            if (pf.split(" ").length == 1)
              // Parent field in the same table, add row index
              pfs[i] = pfs[i].replace(/^x_/, prefix);
          });
          var lfv = lf != "" ? result[lf] : "",
            row = {
              lf: lfv
            };
          dfs.forEach((df, i) => {
            if (df in result) row["df" + (i > 0 ? i + 1 : "")] = result[df];
          });
          ffs.forEach((ff, i) => {
            if (ff in result) row["ff" + (i > 0 ? i + 1 : "")] = result[ff];
          });
          if (lfv && dfs.length > 0 && row["df"]) {
            if (list.ajax === null)
              // Non-Ajax
              list.lookupOptions.push(row);
            var arp = pfs.map(pf => getOptionValues(pf, form)),
              // Get the parent field values
              args = {
                "data": row,
                "parents": arp,
                "valid": true,
                "name": getId(obj),
                "form": form
              };
            $document$1.trigger("addoption", [args]);
            if (args.valid) {
              // Add the new option
              var ar = getOptions(obj),
                txt = newOption(obj, row, form);
              if (obj.options) {
                obj.options[obj.options.length - 1].selected = true;
                if (obj.list) {
                  // Radio/Checkbox list
                  $__default.default(obj.target).find("input").last().trigger("focus");
                }
                if (isAutoSuggest(obj)) {
                  $__default.default(obj).val(lfv).trigger("change");
                  $__default.default(obj.input).val(txt).trigger("focus");
                } else {
                  $__default.default(obj).trigger("change").trigger("focus");
                }
              }
              var $form = $__default.default(form),
                suffix = $form.is("div") ? "_" + form.id : "",
                cnt = $form.find("#" + ew.FORM_KEY_COUNT_NAME + suffix).val();
              if (cnt > 0) {
                // Grid-Add/Edit, update other rows
                for (var i = 1; i <= cnt; i++) {
                  if (i == index) continue;
                  var obj2 = getElements(name.replace(/^x/, "x" + i), form),
                    ar2 = getOptions(obj2);
                  if (JSON.stringify(ar) != JSON.stringify(ar2))
                    // Not same options
                    continue;
                  newOption(obj2, row, form);
                  obj2.render == null ? void 0 : obj2.render(); // Selection list
                }
              }
            }
          }
        }
      } else {
        var _results2;
        // Failure
        if ((_results2 = results) != null && _results2.error) {
          let error = getError(results);
          if (error) _alert(error);
        } else {
          var msg,
            $div = $__default.default("<div></div>").html(data).find("div.ew-message-dialog");
          if ($div[0]) {
            msg = $div.html();
          } else {
            var _results3;
            msg = ((_results3 = results) == null ? void 0 : _results3.failureMessage) || data;
            if (!msg || String(msg).trim() == "") msg = ew.language.phrase("InsertFailed");
          }
          _alert(msg);
        }
      }
    };

    // Fail
    var _fail = function (o) {
      $dlg.modal("hide");
      _alert("Server Error " + o.status + ": " + o.statusText);
    };

    // Submit
    var _submit = async function (e) {
      let $dlg = ew.addOptionDialog,
        form = $dlg.find(".modal-body form")[0],
        frm = forms.get(form.id),
        btn = e == null ? void 0 : e.target,
        $btn = $__default.default(btn);
      if (await frm.canSubmit(e)) {
        $btn.prop("disabled", false).removeClass("disabled");
        document.body.style.cursor = "wait";
        $__default.default.post(getApiUrl([ew.API_ADD_ACTION, form.elements[ew.API_OBJECT_NAME].value]), $__default.default(form).serialize(), _submitSuccess).fail(_fail).always(function () {
          frm.enableForm();
          $btn.prop("disabled", false).removeClass("disabled");
          document.body.style.cursor = "default";
        });
      }
      return false;
    };
    $dlg.modal("hide");
    $dlg.data("args", args);

    // Get form HTML
    var success = function (data) {
      var frm = forms.get(args.lnk),
        prefix = "x_",
        m = args.el.match(/^(x\d+_)/);
      if (m)
        // Contains row index
        prefix = m[1];
      var list = frm.getList(args.el),
        pfs = list.parentFields.slice() // Clone
        .map(pf => pf.split(" ").length == 1 ? pf.replace(/^x_/, prefix) : pf),
        // Parent field in the same table, add row index
        form = frm.htmlForm,
        ar = pfs.map(pf => getOptionValues(pf, form)),
        ar2 = pfs.map(pf => getOptionTexts(pf, form)),
        ffs = list.filterFieldVars.slice(); // Clone
      $dlg.find(".modal-title").html($__default.default(args.lnk).closest(".ew-add-opt-btn").data("title"));
      $dlg.find(".modal-body").html(stripScript(data));
      var form = $dlg.find(".modal-body form")[0];
      if (form) {
        // Set the filter field value
        $__default.default(form).on("keydown", function (e) {
          if (e.key == "Enter" && e.target.nodeName != "TEXTAREA") return _submit();
        });
        // $document.one("updatedone", () => {
        //     ar.forEach((v, i) => {
        //         let obj = getElements(ffs[i], form);
        //         if (obj) {
        //             if (obj.options || obj.length) { // Selection list
        //                 $(obj).first().one("updated", () => selectOption(obj, v));
        //             } else {
        //                 selectOption(obj, v);
        //             }
        //         }
        //     });
        // });
      }

      ew.addOptionDialog = $dlg.modal("show");
      $dlg.find(".modal-footer .btn-primary").click(_submit).focus();
      if (form) {
        // Set the filter field value
        ar.forEach((v, i) => {
          var obj = getElements(ffs[i], form);
          if (obj && v[0]) {
            if (isAutoSuggest(obj)) {
              // AutoSuggest
              obj.value = v[0];
              obj.input.value = ar2[i][0];
              obj.add(v[0], ar2[i][0], true);
            } else if (obj.options || obj.length) {
              // Selection list
              newOption(obj, {
                lf: v[0]
              }, frm, true);
              // $(obj).first().one("updated", () => selectOption(obj, v));
            } else {
              // Text
              obj.value = v[0];
            }
          }
        });
      }
      executeScript(data, args.el, true);
      $dlg.trigger("load.ew"); // Trigger load event
    };

    $__default.default.get(args.url, success).fail(_fail);
    return false;
  }

  // Hide Modal dialog
  function modalDialogHide() {
    let $dlg = $__default.default(this),
      args = $dlg.data("args"),
      frm = $dlg.removeData("args").find(".modal-body form").data("form");
    removeScript("modal_dialog");
    frm == null ? void 0 : frm.destroyEditor();
    $dlg.find(".modal-footer .btn-primary").off();
    $dlg.find(".modal-dialog").removeClass((i, className) => {
      let m = className.match(/table\-\w+/);
      return m ? m[0] : "";
    });
    $dlg.data({
      showing: false,
      url: null
    });
    if (args != null && args.reload) window.location.reload();
  }

  /**
   * Show modal dialog
   *
   * @param {Object} args - Arguments
   * @param {MouseEvent} args.evt - Event
   * @param {HTMLFormElement} args.f - Form of List page
   * @param {HTMLElement} args.lnk - Anchor element
   * @param {string} args.url - URL of content
   * @param {string} args.html - HTML content
   * @param {string|null} args.btn - Button phrase ID
   * @param {boolean} args.footer - Show footer (default true)
   * @param {string} args.caption - Caption in dialog header
   * @param {boolean} args.reload - Reload page after hiding dialog or not
   * @param {string} args.size - Class name of modal dialog 'modal-sm'|'modal-md'|modal-lg'|'modal-xl' (default)
   * @param {Function} args.callback - Callback function (success function after submitting the form)
   * @returns false
   */
  function modalDialogShow(args) {
    var _args$evt3, _bootstrap$Tooltip$ge4, _args$evt4, _args$evt4$currentTar, _args$evt5, _args$evt5$currentTar, _$dlg$data, _$dlg$data2;
    args.lnk || (args.lnk = (_args$evt3 = args.evt) == null ? void 0 : _args$evt3.currentTarget);
    (_bootstrap$Tooltip$ge4 = bootstrap.Tooltip.getInstance(args.lnk)) == null ? void 0 : _bootstrap$Tooltip$ge4.hide();
    let {
        ewAction,
        action
      } = args,
      f = args.f || ((_args$evt4 = args.evt) == null ? void 0 : (_args$evt4$currentTar = _args$evt4.currentTarget) == null ? void 0 : _args$evt4$currentTar.form);
    if ((ewAction == "grid" && action == "edit" || ewAction == "modal" && ["multiedit", "update"].includes(action)) && !keySelected(f)) {
      // Grid-Edit and Multi-Edit/Update
      _alert(ew.language.phrase("NoRecordSelected"));
      return false;
    }
    let url = args.url || ((_args$evt5 = args.evt) == null ? void 0 : (_args$evt5$currentTar = _args$evt5.currentTarget) == null ? void 0 : _args$evt5$currentTar.dataset.url),
      $dlg = ew.modalDialog || $__default.default("#ew-modal-dialog").on("hidden.bs.modal", modalDialogHide); // div#ew-modal-dialog always exists

    if ($dlg.data("showing") && url && $dlg.data("url") == url) return false;
    $dlg.data({
      showing: true,
      url: url
    });
    args.reload = false;

    // Size
    $dlg.find(".modal-dialog").removeClass("modal-sm modal-md modal-lg modal-xl").addClass(args.size || "modal-xl");

    // Caption
    let _caption = function () {
      let args = $dlg.data("args"),
        $lnk = $__default.default(args.lnk);
      return args.caption || $lnk.data("caption") || $lnk.data("original-title") || "";
    };

    // Button text
    let _button = function () {
      let args = $dlg.data("args");
      if (args.btn === null) return "";else if (args.btn && args.btn != "") return ew.language.phrase(args.btn);
      return _caption();
    };

    // Fail
    let _fail = function (o) {
      $dlg.modal("hide");
      if (o.status) _alert("Server Error " + o.status + ": " + o.statusText);
    };

    // Always
    let _always = function () {
      document.body.style.cursor = "default";
    };

    // Check if current page
    let _current = function (url) {
      let a = document.createElement("a");
      a.href = url;
      return window.location.pathname.endsWith(a.pathname);
    };

    // Close hidden dialog
    let _hide = function () {
      if ($dlg.data("showing") && !$dlg.hasClass("show")) $dlg.trigger("hidden.bs.modal"); // Call modalDialogHide()
    };

    /**
     * Handle result
     *
     * @param {Object} result - Result object
     * @param {string|Object} result.error - Error message or object
     * @param {string} result.error.message - Error message
     * @param {string} result.error.description - Error message
     * @param {string} result.failureMessage - Failure message
     * @param {string} result.successMessage - Success message
     * @param {string} result.warningMessage - Warning message
     * @param {string} result.message - Message
     * @param {string} result.url - Redirection URL
     * @param {string} result.modal - Redirect to result.url in current modal dialog
     * @param {boolean} result.view - result.url is View page => No primary button
     * @param {string} result.caption - Caption of modal dialog for result.url
     * @param {boolean} result.reload - Reload current page
     * @param {boolean} result.success - Result => Call callback if function
     */
    let handleResult = function (result) {
      let error = getError(result);
      if (error) {
        _alert(error);
        _hide();
      } else if ($__default.default.isString(result.warningMessage)) {
        _alert(result.warningMessage, cb, "warning");
        _hide();
      } else if ($__default.default.isString(result.message)) {
        _alert(result.message, cb, "body");
        _hide();
      } else if ($__default.default.isString(result.successMessage)) {
        _alert(result.successMessage, cb, "success");
        _hide();
      } else if (result.reload) {
        $dlg.modal("hide");
        window.location.reload();
      } else if ($__default.default.isBoolean(result.success) && isFunction$2(args == null ? void 0 : args.callback)) {
        // If success or failure and callback is function, call the callback
        $dlg.modal("hide");
        args.callback(result);
      } else if (result.url) {
        let url = result.url;
        if (result.modal) {
          if (_current(url)) {
            // Return to current page
            if (!result.error) {
              $dlg.modal("hide");
              refresh(fetch(url));
            }
          } else {
            // Return to other page
            let args = $dlg.data("args");
            args.reload = true;
            args.url = url;
            if (result.caption) args.caption = result.caption;
            args.btn = result.view ? null : "";
            $dlg.data({
              args,
              url
            });
            url = setSearchParams(url, {
              modal: "1",
              rnd: random()
            });
            document.body.style.cursor = "wait";
            $__default.default.get(url).done(success).fail(_fail).always(_always);
          }
        } else {
          $dlg.modal("hide");
          window.location = sanitizeUrl(url);
        }
      }
    };

    // Create buttons
    let _createButtons = function () {
      let $footer = $dlg.find(".modal-footer").empty(),
        // Empty the footer
        $btn = $dlg.find(".card-body button[type=submit]").first().addClass("ew-submit").on("click", _submit); // Find submit button found in card body
      if (!$btn[0]) {
        // No submit button found in card body
        $dlg.find(".modal-body .ew-modal-buttons").contents().clone().find("button").each(function () {
          // Find buttons in <template>
          if (this.type == "submit") {
            this.type = "button"; // Convert to normal button
            this.classList.add("ew-submit");
            if (this.id == "btn-cancel") this.classList.add("ew-cancel"); // Cancel button with type="submit"
            let $this = $__default.default(this);
            $this.on("click", function (e) {
              if (this.dataset.ewAction == "set-action")
                // Set value first
                this.form.elements["action"].value = this.dataset.value;
              $this.data("ajax") === false ? $__default.default(this.form).trigger("submit") : _submit(e);
            });
          } else if (this.id == "btn-cancel") {
            if (this.dataset.href) {
              if (samePath(this.dataset.href, currentUrl.pathname)) this.setAttribute("data-bs-dismiss", "modal");else $__default.default(this).on("click", () => redirect(this.dataset.href));
            } else {
              this.setAttribute("data-bs-dismiss", "modal");
            }
          }
          $footer[0].appendChild(this);
        }); // Clone buttons and append to footer
        if (!$dlg.find(".card-body button[type=submit], .modal-footer .btn")[0]) {
          // No buttons found
          let btn = _button();
          if (btn) {
            $__default.default('<button type="button" class="btn btn-primary ew-btn">' + btn + '</button>').on("click", _submit).appendTo($footer);
            $footer.append('<button type="button" class="btn btn-default ew-btn" data-bs-dismiss="modal">' + ew.language.phrase("CancelBtn") + '</button>');
          } else {
            $footer.append('<button type="button" class="btn btn-default ew-btn" data-bs-dismiss="modal">' + ew.language.phrase("CloseBtn") + '</button>');
          }
        }
        $footer.find("button").on("mousedown", e => e.preventDefault()); // Do not focus button by mouse down
        $footer.find(".btn-primary").focus();
      }
    };

    // Check return URL
    let _return = response => {
      let returnUrl = response.headers.get("Return-Url"); // Check if return URL exists
      if (returnUrl && args.table && ["inline", "modal"].includes(args.ewAction) && !samePath(parseUrl(returnUrl).pathname, currentUrl.pathname)) $__default.default(".ew-grid." + args.table).find("[data-ew-action=modal][data-url='" + returnUrl + "'], [data-ew-action=inline][data-url='" + returnUrl + "']").trigger("click");
    };

    // Submit success
    let _submitSuccess = async function (response) {
      let args = $dlg.data("args"); // Get arguments first or it will be removed by modalDialogHide()
      if (!(response instanceof Response)) return;
      let ct = response.headers.get("Content-Type");
      if (ct != null && ct.includes("json")) {
        let result = await response.json();
        if (isFunction$2(args == null ? void 0 : args.callback)) {
          // If callback is function, call the callback function then return
          let error = getError(result);
          if (error) showToast(error);else $dlg.modal("hide");
          args.callback(result);
          return;
        }
        handleResult(result);
      } else {
        let data = await response.clone().text(),
          body = getContent(data).trim(); // Make sure no leading/trailing space
        if (body.startsWith("<") && body.endsWith(">")) {
          // HTML
          if (args.ajax && args.action == "add" && document.querySelector("main.ew-no-record") && $__default.default(body).find("main:not(.ew-no-record)")[0]) {
            // No existing record, new record added
            $dlg.modal("hide");
            await refresh(Promise.resolve(response));
            _return(response);
            return;
          }
          $dlg.find(".modal-body").html(body);
          $dlg.find(".modal-body .ew-infinite-scroll-grid .ew-grid-middle-panel").css("height", "auto"); // Do not use infinite scrolling height for Grid-Add/Edit
          // If callback is function and no modal buttons, call the callback function then return
          if (!$dlg.find(".modal-body .ew-modal-buttons").length && isFunction$2(args == null ? void 0 : args.callback)) {
            $dlg.modal("hide");
            args.callback(response);
            return;
          }
          _createButtons();
          executeScript(data, "modal_dialog", true);
          $dlg.trigger("load.ew"); // Trigger load event
        } else if (data) {
          $dlg.modal("hide");
          _alert(data);
        }
      }
    };

    // Submit
    let _submit = async function (e) {
      let form = $dlg.find(".modal-body form")[0],
        $form = $__default.default(form),
        frm = forms.get(form.id),
        btn = e == null ? void 0 : e.target;
      if (btn.classList.contains("disabled")) return false;
      if (btn.classList.contains("ew-cancel")) {
        // Cancel button with type="submit"
        $__default.default.post($form.attr("action"), $form.serialize(), success).fail(_fail).always(_always);
      } else if (await frm.canSubmit(e)) {
        document.body.style.cursor = "wait";
        let p = _fetch($form.attr("action"), {
          method: "POST",
          body: $form.serialize()
        }).catch(_fail).finally(() => {
          frm.enableForm();
          _always();
        });
        if (frm.submitWithFetch) {
          p.then(async response => {
            var _resp$headers$get;
            // Check error before refresh
            let resp = response.clone(),
              isJson = (_resp$headers$get = resp.headers.get("Content-Type")) == null ? void 0 : _resp$headers$get.includes("json"),
              result = isJson ? await resp.json() : null,
              hasErrors = (result == null ? void 0 : result.validation) || getError(result);
            if (!isJson || !hasErrors)
              // HTML or JSON without error => close dialog
              $dlg.modal("hide");
            let args = {
                form,
                result: Promise.resolve(response),
                context: hasErrors ? "#ew-modal-dialog.show .modal-body" : null
              },
              evt = $__default.default.Event("aftersubmit", {
                originalEvent: e
              });
            frm.trigger(evt, [args]);
          });
        } else {
          p.then(_submitSuccess);
        }
      }
      return false;
    };
    let success = function (data) {
      let result = parseJson(data);
      if ($__default.default.isObject(result)) {
        handleResult(result);
      } else {
        let args = $dlg.data("args"),
          $lnk = $__default.default(args.lnk),
          body = getContent(data);
        $dlg.find(".modal-title").html(_caption());
        $dlg.find(".modal-body").html(body);
        $dlg.find(".modal-body .ew-infinite-scroll-grid .ew-grid-middle-panel").css("height", "auto"); // Do not use infinite scrolling height for Grid-Add/Edit
        _createButtons();
        $dlg.find(".modal-footer").toggle(args.footer !== false);
        let lnkData = $lnk.data() || {};
        if (lnkData.table) $dlg.find(".modal-dialog").addClass("table-" + lnkData.table);
        $dlg.find(".modal-body form").on("keydown", e => {
          if (e.key == "Enter" && e.target.nodeName != "TEXTAREA") {
            $dlg.find(".modal-footer .btn.btn-primary").trigger("click");
            return false;
          }
        });
        if (lnkData.ajax && lnkData.table && lnkData.action == "view") {
          // Modal View page and use Ajax actions
          [".ew-add", ".ew-copy", ".ew-edit", ".ew-delete"].forEach(className => {
            let $dlglnk = $dlg.find(".modal-body " + className + "[data-ew-action=modal]"),
              // Link inside modal dialog
              $lnk = $__default.default("[data-ew-action=modal][data-table='" + lnkData.table + "'][data-url='" + $dlglnk.data("url") + "']").not(".modal-body *").first(); // Link outside modal dialog
            $lnk[0] ? $dlglnk.on("click", e => {
              e.preventDefault();
              e.stopImmediatePropagation();
              $lnk.trigger("click"); // Click the link outside
            }) : $dlglnk.addClass("d-none"); // Hide the link
          });
        }

        ew.modalDialog = $dlg.modal("show");
        executeScript(data, "modal_dialog", true);
        // Fix for CKEditor
        let modal = bootstrap.Modal.getInstance($dlg[0]);
        if (!modal._focustrap.__handleFocusin) {
          modal._focustrap.__handleFocusin = modal._focustrap._handleFocusin;
          modal._focustrap._handleFocusin = function (e) {
            var _e$target10;
            // Use function for "this"
            if ((_e$target10 = e.target) != null && _e$target10.matches("[class^=cke_dialog_]"))
              // Element from CKEditor dialog
              return; // Do not focus the modal
            this.__handleFocusin(e);
          };
        }
        $dlg.trigger("load.ew"); // Trigger load event
      }
    };

    let show = function (html) {
      let args = $dlg.data("args");
      $dlg.find(".modal-title").html(_caption());
      $dlg.find(".modal-body").html(html);
      _createButtons();
      $dlg.find(".modal-footer").toggle(args.footer !== false);
      ew.modalDialog = $dlg.modal("show");
    };

    // Restore previous callback
    if (args.callback === true && (_$dlg$data = $dlg.data("args")) != null && _$dlg$data.callback) args.callback = $dlg.data("args").callback;
    if (url && (_$dlg$data2 = $dlg.data("args")) != null && _$dlg$data2.reload) {
      // About to load URL but previous result required reload
      args.reload = true; // Carry the reload state to current args
      $dlg.data("args").reload = false; // Remove reload so "hidden" event will not reload
    }

    $dlg.modal("hide");
    $dlg.data("args", args);
    if (url) {
      document.body.style.cursor = "wait";
      let params = ew.getSearchParams(url);
      params.set("rnd", random());
      if (params.get("action") != "delete") params.set("modal", "1");
      $__default.default.ajax({
        url: setSearchParams(url, params),
        method: f ? "POST" : "GET",
        data: f ? $__default.default(f).serialize() : "",
        headers: args.json ? {
          Accept: "application/json, */*; q=0.01"
        } : {} // JSON response
      }).done(success).fail(_fail).always(_always);
    } else if (args.html) {
      show(args.html);
    }
    return false;
  }

  /**
   * Show dialog for import
   *
   * @param {Object} args - Arguments
   * @param {string} args.hdr - Dialog header
   * @param {HTMLElement} args.lnk - Anchor element
   * @returns
   */
  function importDialogShow(args) {
    var _args$evt6, _bootstrap$Tooltip$ge5;
    args.lnk = args.lnk || ((_args$evt6 = args.evt) == null ? void 0 : _args$evt6.currentTarget);
    (_bootstrap$Tooltip$ge5 = bootstrap.Tooltip.getInstance(args.lnk)) == null ? void 0 : _bootstrap$Tooltip$ge5.hide();
    let $dlg = ew.importDialog || $__default.default("#ew-import-dialog");
    if (!$dlg[0]) {
      _alert("DIV #ew-import-dialog not found.");
      return false;
    }
    let $input = $dlg.find("#importfiles"),
      $dropzone = $input.closest(".ew-file-drop-zone"),
      $bd = $dlg.find(".modal-body"),
      $data = $bd.find(":input[id!=importfiles]"),
      $message = $bd.find(".message"),
      $progress = $bd.find(".progress"),
      table,
      sse;

    // Add row to table
    let addRow = function (result) {
      let {
          row,
          success,
          error,
          count
        } = result,
        callback = () => {
          table.addRow(row).then(r => r.getElement().classList.toggle("text-danger", error));
          if (error) console.log(ew.language.phrase("ImportRowError").replace("%i", count + " " + JSON.stringify(row)).replace("%e", error));
        };
      row = {
        "_index": count,
        "_success": success,
        "_error": error,
        ...row
      };
      if (!table) {
        table = new Tabulator("#ew-import-dialog .result", $__default.default.extend({
          data: [row],
          index: "_index",
          autoColumns: true,
          autoColumnsDefinitions: definitions => {
            definitions.forEach(column => {
              column.headerSort = false;
              column.resizable = false;
              if (["_index", "_error"].includes(column.field)) {
                column.visible = false;
              } else if (column.field == "_success") {
                column.title = "#";
                column.headerSort = true;
                column.headerSortTristate = true;
                column.formatter = cell => {
                  cell.getElement().style.textOverflow = "clip";
                  let row = cell.getRow(),
                    data = row.getData();
                  if (data._error) new bootstrap.Tooltip(row.getElement(), {
                    placement: "bottom",
                    title: data._error,
                    container: "#ew-import-dialog .result"
                  });
                  return (data._success ? '<i class="fa-solid fa-check text-success"></i>' : '<i class="fa-solid fa-xmark text-danger"></i>') + " #" + data._index;
                };
              }
            });
            return definitions;
          }
        }, ew.importTabulatorOptions));
        table.on("tableBuilt", () => table.element.querySelector(".tabulator-row").classList.toggle("text-danger", error));
      } else {
        table.initialized ? callback() : table.on("tableBuilt", callback);
      }
    };

    // Enable buttons
    let enableButtons = enabled => $dlg.find(".modal-footer .btn").prop("disabled", !enabled);

    // Show message
    let showMessage = function (msg, classname) {
      let $msg = $__default.default("<div>" + msg + "</div>");
      if (classname) $msg.addClass(classname);
      $message.removeClass("d-none").html($msg);
      if (classname == "text-danger") enableButtons(true);
    };

    // Hide message
    let hideMessage = () => $message.addClass("d-none").html("");

    // Show progress
    let showProgress = (pc, classname) => $progress.removeClass("d-none").find(".progress-bar").removeClass("bg-success bg-info").addClass(classname || "bg-success").attr("aria-valuenow", pc).css("width", pc + "%").html(pc + "%");

    // Hide progress
    let hideProgress = () => $progress.addClass("d-none").find(".progress-bar").attr("aria-valuenow", 0).css("width", "0%").html("0%");

    // Upload progress
    let uploadProgress = function (data) {
      const pc = parseInt(100 * data.loaded / data.total, 10);
      showProgress(pc, "bg-primary");
      if (pc === 100) showMessage(ew.language.phrase("ImportUploadComplete"), "text-primary");else showMessage(ew.language.phrase("ImportUploadProgress").replace("%p", pc), "text-primary");
    };

    // Update import progress
    let updateProgress = function (e) {
      const result = parseJson(e.data) || {};
      try {
        let tcnt = result.totalCount || 0,
          cnt = result.count || 0,
          scnt = result.successCount || 0,
          fcnt = result.failCount || 0,
          filename = result.file;
        if (tcnt > 0 && $dlg.find(".modal-footer .ew-close-btn").data("import-progress")) {
          // Show progress
          if (result.row) addRow(result);
          let pc = parseInt(100 * cnt / tcnt, 10);
          showProgress(pc);
          showMessage(ew.language.phrase("ImportResult").replace("%t", tcnt).replace("%c", cnt).replace("%f", filename).replace("%s", scnt).replace("%e", fcnt), fcnt > 0 ? "text-danger" : "text-primary");
        }
      } catch (e) {}
    };

    // Import complete
    let importComplete = function (e) {
      var _sse;
      let msg = [],
        result = parseJson(e.data) || {},
        fileResults = result.files;
      showProgress(100);
      $dlg.find(".modal-footer .ew-close-btn").data("import-progress", false); // Stop import progress
      if (Array.isArray(fileResults)) {
        fileResults.forEach(fileResult => {
          let tcnt = fileResult.totalCount || 0,
            cnt = fileResult.count || 0,
            scnt = fileResult.successCount || 0,
            fcnt = fileResult.failCount || 0;
          msg.push(ew.language.phrase("ImportResult").replace("%t", tcnt).replace("%c", cnt).replace("%f", fileResult.file).replace("%s", scnt).replace("%e", fcnt));
        });
      }
      if (result.success) {
        if (result.rollbacked) msg.push(ew.language.phrase("ImportSave"));
        showMessage(msg.join("<br>"), "text-success");
        $dlg.find(".modal-footer .ew-close-btn").data("imported", true);
      } else {
        if (ew.IMPORT_MAX_FAILURES > 0 && result.failCount >= ew.IMPORT_MAX_FAILURES) msg.push(ew.language.phrase("ImportMaxFailuresExceeded").replace("%s", ew.IMPORT_MAX_FAILURES));
        showMessage(msg.join("<br>"), "text-danger"); // Show error message
      }
      // hideProgress();
      document.body.style.cursor = "default";
      $dlg.find(".modal-footer .ew-close-btn").prop("disabled", false); // Enable the close button
      let $btn = $dlg.find(".modal-footer .ew-import-btn").toggleClass("d-none", !result.rollbacked || !result.success); // Show/Hide the import button
      if (result.rollbacked) $btn.off("click").one("click", saveImport);
      console.log("Closing EventSource...");
      (_sse = sse) == null ? void 0 : _sse.close();
    };

    // Import fail
    let importFail = function (e) {
      var _sse2;
      document.body.style.cursor = "default";
      $dlg.find(".modal-footer .ew-close-btn").data("import-progress", false); // Stop import progress
      let result = parseJson(e.data) || {};
      if (result.error) showMessage(result.error, "text-danger");
      console.log("Closing EventSource...");
      (_sse2 = sse) == null ? void 0 : _sse2.close();
    };

    // Import file
    let importFiles = function (filetoken) {
      var _table;
      document.body.style.cursor = "wait";
      showProgress(0);
      (_table = table) == null ? void 0 : _table.clearData();
      $input.data(ew.API_FILE_TOKEN_NAME, filetoken);
      $dlg.find(".modal-footer .ew-close-btn").data("import-progress", true); // Show import progress
      let params = new URLSearchParams($data.serialize());
      params.set(ew.API_ACTION_NAME, ew.API_IMPORT_ACTION);
      params.set(ew.API_FILE_TOKEN_NAME, filetoken);
      params.set("rollback", "1");
      sse = new EventSource(currentPage() + "?" + params.toString());
      sse.addEventListener("message", updateProgress);
      sse.addEventListener("error", importFail);
      sse.addEventListener("complete", importComplete);
    };

    // Save import
    let saveImport = function (e) {
      var _table2;
      document.body.style.cursor = "wait";
      showProgress(0);
      (_table2 = table) == null ? void 0 : _table2.clearData();
      $dlg.find(".modal-footer .ew-close-btn").data("import-progress", true); // Show import progress
      let params = new URLSearchParams($data.serialize());
      params.set(ew.API_ACTION_NAME, ew.API_IMPORT_ACTION);
      params.set(ew.API_FILE_TOKEN_NAME, $input.data(ew.API_FILE_TOKEN_NAME));
      params.set("rollback", "0");
      sse = new EventSource(currentPage() + "?" + params.toString());
      sse.addEventListener("message", updateProgress);
      sse.addEventListener("error", importFail);
      sse.addEventListener("complete", importComplete);
    };
    let options = ew.importUploadOptions;
    if (!options.acceptFileTypes) options.acceptFileTypes = new RegExp('\\.(' + ew.IMPORT_FILE_ALLOWED_EXTENSIONS.replace(/,/g, '|') + ')$', 'i');
    if (!$input.data("blueimpFileupload")) {
      $input.fileupload(Object.assign({
        url: getApiUrl(ew.API_UPLOAD_ACTION),
        dataType: "json",
        autoUpload: true,
        singleFileUploads: false,
        dropZone: $dropzone,
        messages: {
          acceptFileTypes: ew.language.phrase("UploadErrorAcceptFileTypes"),
          maxFileSize: ew.language.phrase("UploadErrorMaxFileSize"),
          maxNumberOfFiles: ew.language.phrase("UploadErrorMaxNumberOfFiles"),
          minFileSize: ew.language.phrase("UploadErrorMinFileSize")
        },
        beforeSend: function (jqxhr, settings) {
          settings.data.set("session", ew.SESSION_ID);
          settings.data.set(ew.TOKEN_NAME_KEY, ew.TOKEN_NAME); // Add token name for $.ajax() sent by jQuery File Upload (not by ajaxSend) // PHP
          settings.data.set(ew.ANTIFORGERY_TOKEN_KEY, ew.ANTIFORGERY_TOKEN); // Add antiforgery token for $.ajax() sent by jQuery File Upload (not by ajaxSend) // PHP
          if (ew.API_JWT_TOKEN && !ew.IS_WINDOWS_AUTHENTICATION)
            // Do NOT set JWT authorization header if Windows Authentication
            jqxhr.setRequestHeader(ew.API_JWT_AUTHORIZATION_HEADER, "Bearer " + ew.API_JWT_TOKEN);
        },
        done: function (e, data) {
          var _data$result, _data$result$files;
          if (Array.isArray(data == null ? void 0 : (_data$result = data.result) == null ? void 0 : (_data$result$files = _data$result.files) == null ? void 0 : _data$result$files.importfiles)) {
            let ok = true;
            data.result.files.importfiles.forEach(function (file, index) {
              if (file.error) {
                showMessage(ew.language.phrase("ImportUploadError").replace("%f", file.name).replace("%s", file.error), "text-danger");
                ok = false;
              }
            }); // Show upload errors for each file
            if (ok) importFiles(data.result[ew.API_FILE_TOKEN_NAME]); // Import uploaded files
          }
        },

        change: function (e, data) {
          hideMessage();
        },
        processfail: function (e, data) {
          data.files.forEach(function (file, index) {
            if (file.error) showMessage(ew.language.phrase("ImportUploadError").replace("%f", file.name).replace("%s", file.error), "text-danger");
          }); // Show process errors for each file
        },

        fail: function (e, data) {
          showMessage(ew.language.phrase("ImportServerError").replace("%s", data.textStatus).replace("%t", data.errorThrown), "text-danger");
        },
        progressall: function (e, data) {
          uploadProgress(data);
        }
      }, options));
    }
    $dlg.modal("hide").find(".modal-title").html(args.hdr);
    $dlg.find(".modal-footer .ew-close-btn").off("click.ew").on("click.ew", function () {
      let $this = $__default.default(this);
      if ($this.data("imported")) {
        $this.data("imported", false);
        window.location.reload();
      }
    });
    hideMessage();
    ew.importDialog = $dlg.on("show.bs.modal", function () {
      var _this$querySelector, _table3, _table4;
      hideProgress();
      (_this$querySelector = this.querySelector(".modal-footer .ew-import-btn")) == null ? void 0 : _this$querySelector.classList.add("d-none");
      (_table3 = table) == null ? void 0 : _table3.element.removeAttribute("style");
      (_table4 = table) == null ? void 0 : _table4.destroy();
      table = null;
    }).modal("show");
    return false;
  }

  // Auto-fill
  function autoFill(el) {
    let f = forms.get(el).$element[0];
    if (!f) return;
    let ar = getOptionValues(el),
      id = getId(el),
      m = id.match(/^([xy])(\d*)_/),
      rowindex = m ? m[2] : "",
      list = forms.get(el).getList(id),
      dest_array = list.autoFillTargetFields;
    let success = function (data) {
      let results = data == null ? void 0 : data.records,
        result = Array.isArray(results) && results.length > 0 ? results[0] : [];
      for (let j = 0; j < dest_array.length; j++) {
        let destEl = getElements(dest_array[j].replace(/^x_/, "x" + rowindex + "_"), f);
        if (destEl) {
          let val = $__default.default.isValue(result["af" + j]) ? String(result["af" + j]) : "",
            args = {
              results,
              result,
              data: val,
              form: f,
              name: id,
              target: dest_array[j],
              cancel: false,
              trigger: true
            };
          $__default.default(el).trigger("autofill", [args]); // Fire event
          if (args.cancel) continue;
          val = args.data; // Process the value
          if (destEl.options) {
            // Selection list
            selectOption(destEl, val.split(","));
            if (isAutoSuggest(destEl)) {
              // Auto-Suggest
              destEl.input.value = val;
              updateOptions.call(forms.get(f.id), destEl);
            }
          } else if (isHiddenTextArea(destEl)) {
            // HTML editor
            destEl.value = val;
            $__default.default(destEl).data("editor").set();
          } else if (destEl.type == "checkbox") {
            // Boolean checkbox
            destEl.checked = convertToBool(val);
          } else {
            destEl.value = val;
          }
          if (args.trigger) $__default.default(destEl).trigger("change");
        }
      }
      return result;
    };
    if (ar.length > 0 && ar[0] != "") {
      let data = Object.assign({
        page: list.page,
        field: list.field,
        ajax: "autofill",
        v0: ar[0],
        language: ew.LANGUAGE_ID
      }, getUserParams('#p_' + id, f));
      // Add parent field values
      let parentId = list.parentFields.slice(); // Clone
      if (rowindex != "") {
        for (let i = 0, len = parentId.length; i < len; i++) {
          let ar = parentId[i].split(" ");
          if (ar.length == 1)
            // Parent field in the same table, add row index
            parentId[i] = parentId[i].replace(/^x_/, "x" + rowindex + "_");
        }
      }
      let arp = parentId.map(pid => getOptionValue(pid, f)); // Get parent field values
      arp.forEach((p, i) => data["v" + (i + 1)] = p); // Filter by parent fields
      return $__default.default.post(getApiUrl(ew.API_LOOKUP_ACTION), data, success, "json");
    }
    return success();
  }

  // Set up tooltip links as popovers
  function tooltip(i, el) {
    var $this = $__default.default(el),
      $tt = $__default.default("#" + $this.data("tooltip-id")),
      trig = $this.data("trigger") || "hover",
      dir = $this.data("placement") || "auto";
    if (!$tt[0] || $tt.text().trim() == "" && !$tt.find("img[src!='']")[0]) return;
    if (!bootstrap.Popover.getInstance(el)) {
      $this.popover({
        ...ew.popoverOptions,
        placement: dir,
        trigger: trig,
        delay: 100,
        container: document.getElementById("ew-tooltip"),
        content: $tt.html()
      }).on("show.bs.popover", function () {
        var wd = $this.data("tooltip-width");
        if (wd)
          // Set width before show
          $__default.default(bootstrap.Popover.getInstance(this)._getTipElement()).css("max-width", parseInt(wd, 10) + "px");
      });
    }
  }

  /**
   * Init search filters
   */
  function initSearchFilters(e) {
    var _e$target11;
    let el = (_e$target11 = e == null ? void 0 : e.target) != null ? _e$target11 : document,
      btn = el == null ? void 0 : el.querySelector(".ew-filter-option");
    if (ew.CLIENT_SEARCH_FILTER || ew.SERVER_SEARCH_FILTER && ew.IS_LOGGEDIN && !ew.IS_SYS_ADMIN && ew.CURRENT_USER_NAME != "") {
      $__default.default(btn).removeClass("d-none").find(".ew-btn-dropdown").on("show.bs.dropdown", function (e) {
        var _frm$filterList$filte, _frm$filterList;
        let formId = $__default.default(this).find(".ew-save-filter[data-form], .ew-delete-filter[data-form]").data("form"),
          frm = ew.forms.get(formId),
          filters = ew.CLIENT_SEARCH_FILTER ? ew.parseJson(localStorage.getItem(ew.PROJECT_NAME + "_" + frm.id + "_filters") || "[]") || [] : (_frm$filterList$filte = frm == null ? void 0 : (_frm$filterList = frm.filterList) == null ? void 0 : _frm$filterList.filters) != null ? _frm$filterList$filte : [];
        frm.setupFilters(e, filters.filter(val => Array.isArray(val) && val.length == 2));
      });
    } else {
      btn == null ? void 0 : btn.classList.add("d-none");
    }
  }

  /**
   * Show dialog for email sending
   *
   * @param {Object} args - Arguments
   * @param {MouseEvent} args.evt - Event
   * @param {string} args.hdr - Dialog header
   * @param {Object} args.key - Key as object
   * @param {boolean} args.exportSelected - Exported selected only
   * @param {string} args.url - URL of content (for Custom Template)
   * @param {string} args.exportId - Export ID (for Custom Template)
   * @returns false
   */
  function emailDialogShow(args) {
    let $dlg = ew.emailDialog || $__default.default("#ew-email-dialog").on("shown.bs.modal", e => setTimeout(() => {
      var _e$target$querySelect;
      return (_e$target$querySelect = e.target.querySelector(".modal-body .form-control")) == null ? void 0 : _e$target$querySelect.focus();
    }, 200)).on("click", ".modal-footer .btn-primary", function (e) {
      var _$$closest$find$data;
      e.preventDefault();
      if ((_$$closest$find$data = $__default.default(this).closest(".modal").find(".modal-body form").data("form")) != null && _$$closest$find$data.submit()) $dlg.modal("hide");
    });
    if (!$dlg[0]) {
      _alert("DIV #ew-email-dialog not found.");
      return false;
    }
    let form = args.evt.currentTarget.form;
    if (args.exportSelected && !keySelected(form)) {
      _alert(ew.language.phrase("NoRecordSelected"));
      return false;
    }
    let $f = $dlg.find(".modal-body form"),
      frm = $f.data("form");
    if (!frm) {
      frm = new FormBuilder().setId($f.attr("id")).addFields([["sender", [ew.Validators.required(ew.language.phrase("Sender")), ew.Validators.email]], ["recipient", [ew.Validators.required(ew.language.phrase("Recipient")), ew.Validators.emails(ew.MAX_EMAIL_RECIPIENT, ew.language.phrase("EnterProperRecipientEmail"))]], ["cc", ew.Validators.emails(ew.MAX_EMAIL_RECIPIENT, ew.language.phrase("EnterProperCcEmail"))], ["bcc", ew.Validators.emails(ew.MAX_EMAIL_RECIPIENT, ew.language.phrase("EnterProperBccEmail"))], ["subject", ew.Validators.required(ew.language.phrase("Subject"))]]).setValidate(function () {
        return this.validateFields();
      }).setSubmit(function () {
        if (!this.validate()) return false;
        let data = mergeSearchParams($f.serialize(), args.key, getchartParams(), form && args.exportSelected ? $__default.default(form).find("input[type=checkbox][name='key_m[]']:checked").serialize() : null); // Export selected
        $dlg.modal("hide");
        args.custom // Custom Template
        ? _export({
          ...args,
          ...{
            export: "email",
            custom: true,
            emailForm: $f[0]
          }
        }) : exportEmail($f.find("#tblvar").val(), data);
        return false;
      }).build();
      $f.data("form", frm);
    }
    ew.emailDialog = $dlg.modal("hide").find(".modal-title").html(args.hdr).end().modal("show");
    return false;
  }

  // Export to email
  function exportEmail(tblVar, data) {
    let swal = window.parent.Swal; // Use window.parent.Swal in case in iframe
    data = mergeSearchParams(data, [[ew.TOKEN_NAME_KEY, ew.TOKEN_NAME], [ew.ANTIFORGERY_TOKEN_KEY, ew.ANTIFORGERY_TOKEN]]);
    return swal.fire({
      ...ew.sweetAlertSettings,
      showConfirmButton: false,
      html: "<p>" + ew.language.phrase("Exporting") + "</p>",
      allowOutsideClick: false,
      allowEscapeKey: false,
      willOpen: () => {
        swal.showLoading();
        _fetch(getApiUrl([ew.API_EXPORT_ACTION, "email", tblVar]) + "?rnd=" + random(), {
          // Post to Export API
          method: "POST",
          body: data
        }).then(async response => {
          swal.hideLoading();
          let result = await response.json();
          if (result != null && result.success) {
            swal.update({
              html: "<div class='text-success'>" + result.message + "</div>"
            });
            $document$1.trigger("export", [{
              type: "done",
              tblVar,
              data,
              result
            }]);
          } else {
            var _ref3;
            swal.update({
              html: (_ref3 = "<div class='text-danger'>" + getError(result)) != null ? _ref3 : ew.language.phrase("FailedToExport") + "</div>"
            });
            $document$1.trigger("export", [{
              type: "fail",
              tblVar,
              data,
              result
            }]);
          }
        }).catch(error => {
          var _ref4;
          swal.hideLoading();
          swal.update({
            html: (_ref4 = "<div class='text-danger'>" + error) != null ? _ref4 : ew.language.phrase("FailedToExport") + "</div>"
          });
          $document$1.trigger("export", [{
            type: "fail",
            tblVar,
            data,
            error
          }]);
        }).finally(() => {
          swal.update({
            showConfirmButton: true
          });
          $document$1.trigger("export", [{
            type: "always",
            tblVar,
            data
          }]);
        });
      }
    });
  }

  // Show drill down
  function showDrillDown(e, obj, url, id, hdr) {
    if (e != null && e.ctrlKey) {
      let newUrl = setSearchParams(url, {
        d: "2"
      }); // Change "d" parameter to 2
      return redirect(newUrl);
    }
    var $obj = $__default.default(obj);
    var pos = $obj.data("drilldown-placement") || "auto";
    var args = {
      obj: $obj[0],
      placement: pos,
      id,
      url,
      hdr
    };
    $document$1.trigger("drilldown", [args]);
    var ar = args.url.split("?");
    args.file = ar[0] || "";
    args.data = ar[1] || "";
    if (!bootstrap.Popover.getInstance(obj)) {
      $obj.popover({
        ...ew.popoverOptions,
        placement: args.placement,
        trigger: "manual",
        title: args.hdr,
        template: '<div class="popover" role="tooltip"><h3 class="popover-header d-none" style="cursor: move;"></h3><div class="popover-body"></div></div>',
        // No .popover-arrow
        content: ew.spinnerTemplate(),
        container: $__default.default("#ew-drilldown-panel").draggable(ew.draggableOptions),
        boundary: "viewport"
      }).on("show.bs.popover", function (e) {
        $obj.attr("data-original-title", "");
      }).on("shown.bs.popover", function (e) {
        if (!$obj.data("args")) return;
        var data = $obj.data("args").data;
        $__default.default.ajax({
          cache: false,
          dataType: "html",
          type: "POST",
          data: data,
          url: $obj.data("args").file,
          success: function (data) {
            var $tip = $__default.default(bootstrap.Popover.getInstance(obj)._getTipElement());
            if (args.hdr) $tip.find(".popover-header").empty().removeClass("d-none").append('<button type="button" class="btn-close" aria-label="' + ew.language.phrase("CloseBtn") + '"></button>' + args.hdr).find(".btn-close").on("click", function () {
              $obj.popover("hide");
            });
            var m = data.match(/<body[^>]*>([\s\S]*?)<\/body\s*>/i); // Use HTML in document body only
            data = m ? m[0] : data;
            var html = ew.stripScript(data);
            $tip.find(".popover-body").html($__default.default("<div></div>").html(html).find("#ew-report")) // Insert the container table only
            .find(".ew-table").each(ew.setupTable);
            ew.executeScript(data, id, true);
            $obj.popover("update");
          },
          error: function (o) {
            if (o.responseText) {
              let popover = bootstrap.Popover.getInstance(obj);
              if ($__default.default.isString(o.responseText) && o.responseText.startsWith("{") && o.responseText.endsWith("}")) {
                var _result$error12, _result$error13;
                var result = parseJson(o.responseText);
                if (result != null && (_result$error12 = result.error) != null && _result$error12.type && result != null && (_result$error13 = result.error) != null && _result$error13.description) {
                  popover == null ? void 0 : popover.hide();
                  return _alert({
                    title: result.error.type,
                    html: result.error.description,
                    customClass: {
                      title: "ew-swal2-title text-danger",
                      htmlContainer: "ew-swal2-html-container text-danger"
                    }
                  });
                }
              }
              popover == null ? void 0 : popover.setContent({
                ".popover-body": '<p class="text-danger">' + o.responseText + '</p>'
              });
            }
          }
        });
      }).on("hidden.bs.popover", function (e) {
        ew.removeScript(id);
      });
    }
    $obj.data("args", args).popover("show");
  }

  /**
   * Ajax query
   * @param {Object} data - Object to passed to API
   * @param {callback} callback - Callback function for async request (see http://api.jquery.com/jQuery.post/), empty for sync request
   * @returns {string|string[]}
   */
  function ajax(data, callback) {
    if (!$__default.default.isObject(data) || !data.url && !data.action) return undefined;
    var action;
    if (data.url) {
      if (data.url.startsWith(getApiUrl())) action = data.url.replace(getApiUrl(), "").split("/")[0];else if (data.url.startsWith(ew.API_URL)) action = data.url.replace(ew.API_URL, "").split("/")[0];
    } else {
      action = data.action;
      delete data.action;
    }
    var obj = Object.assign({}, data);
    var _convert = response => {
      if ($__default.default.isObject(response) && response.result == "OK") {
        var results = response.records;
        if (Array.isArray(results) && results.length == 1) {
          // Single row
          results = results[0];
          if (Array.isArray(results) && results.length == 1)
            // Single column
            return results[0]; // Return a value
          else return results; // Return a row
        }

        return results;
      }
      return response;
    };
    var url = obj.url || getApiUrl(action),
      // URL
      type = obj.type || ([ew.API_LIST_ACTION, ew.API_VIEW_ACTION, ew.API_DELETE_ACTION].includes(action) ? "GET" : "POST");
    delete obj.url;
    delete obj.type;
    obj.dataType = "json";
    if (isFunction$2(callback)) {
      // Async
      $__default.default.ajax({
        url: url,
        type: type,
        data: obj,
        success: function (response) {
          callback(_convert(response));
        }
      });
    } else {
      // Sync
      var response = $__default.default.ajax({
        url: url,
        async: false,
        type: type,
        data: obj
      });
      return _convert(response.responseJSON);
    }
  }

  // Get URL of current page
  function currentPage() {
    return location.href.split("#")[0].split("?")[0];
  }

  // Toggle search operator
  function toggleSearchOperator(e, id, value) {
    var el = e.currentTarget.form.elements[id];
    if (!el) return;
    el.value = el.value != value ? value : "=";
  }

  // Toggle multi-column layout
  function toggleLayout(el) {
    var _bootstrap$Tooltip$ge6;
    (_bootstrap$Tooltip$ge6 = bootstrap.Tooltip.getInstance(el)) == null ? void 0 : _bootstrap$Tooltip$ge6.hide();
    document.body.style.cursor = "wait";
    fetch(sanitizeUrl(setLayout(el.dataset.url, el.dataset.layout))).then(response => response.text()).then(html => {
      let $grid = $__default.default(".ew-multi-column-grid").html($__default.default("<div>" + html + "</div>").find(".ew-multi-column-grid").html());
      ew.initPage({
        target: $grid[0]
      });
      $grid.trigger("load.ew");
    }).catch(error => _alert(error)).finally(() => document.body.style.cursor = "default");
  }

  // Copy inner text to clipboard
  function copyToClipboard(source) {
    var _source, _source2;
    source = $__default.default.isString(source) ? document.querySelector(source) : source;
    const str = ((_source = source) == null ? void 0 : _source.value) || ((_source2 = source) == null ? void 0 : _source2.innerText);
    if (str) {
      const el = document.createElement("textarea");
      el.value = str;
      el.setAttribute("readonly", "");
      el.style.position = "absolute";
      el.style.left = "-9999px";
      document.body.appendChild(el);
      el.select();
      document.execCommand("copy");
      document.body.removeChild(el);
    }
    return str;
  }

  /**
   * Validators
   */

  // Check integer
  function checkInteger(object_value) {
    if (!object_value || object_value.length == 0) return true;
    if (object_value.includes(ew.DECIMAL_SEPARATOR)) return false;
    return checkNumber(object_value);
  }

  // Check number
  function checkNumber(object_value) {
    object_value = String(object_value);
    if (!object_value || object_value.length == 0) return true;
    object_value = object_value.trim();
    // let re = new RegExp("^[+\-\d\s%" + escapeRegExChars(ew.DECIMAL_SEPARATOR) + escapeRegExChars(ew.GROUPING_SEPARATOR) + ew.CURRENCY_SYMBOL + "]+$");
    // return re.test(object_value) && ew.parseNumber(object_value) !== null;
    return ew.parseNumber(object_value) !== null;
  }

  // Escape regular expression chars
  function escapeRegExChars(str) {
    return str.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
  }

  // Check range
  function checkRange(object_value, min_value, max_value) {
    if (!object_value || object_value.length == 0) return true;
    if ($__default.default.isNumber(min_value) || $__default.default.isNumber(max_value)) {
      // Number
      if (checkNumber(object_value)) object_value = ew.parseNumber(object_value);
    }
    if (min_value !== null && object_value < min_value) return false;
    if (max_value !== null && object_value > max_value) return false;
    return true;
  }

  // Check phone
  function checkPhone(object_value) {
    if (!object_value || object_value.length == 0) return true;
    return /^\(\d{3}\) ?\d{3}( |-)?\d{4}|^\d{3}( |-)?\d{3}( |-)?\d{4}$/.test(object_value.trim());
  }

  // Check zip
  function checkZip(object_value) {
    if (!object_value || object_value.length == 0) return true;
    return /^\d{5}$|^\d{5}-\d{4}$/.test(object_value.trim());
  }

  // Check credit card
  function checkCreditCard(object_value) {
    if (!object_value || object_value.length == 0) return true;
    var creditcard_string = object_value.replace(/\D/g, "");
    if (creditcard_string.length == 0) return false;
    var doubledigit = creditcard_string.length % 2 == 1 ? false : true;
    var tempdigit,
      checkdigit = 0;
    for (var i = 0, len = creditcard_string.length; i < len; i++) {
      tempdigit = parseInt(creditcard_string.charAt(i), 10);
      if (doubledigit) {
        tempdigit *= 2;
        checkdigit += tempdigit % 10;
        if (tempdigit / 10 >= 1.0) checkdigit++;
        doubledigit = false;
      } else {
        checkdigit += tempdigit;
        doubledigit = true;
      }
    }
    return checkdigit % 10 == 0;
  }

  // Check social security number
  function checkSsn(object_value) {
    if (!object_value || object_value.length == 0) return true;
    return /^(?!000)([0-6]\d{2}|7([0-6]\d|7[012]))([ -]?)(?!00)\d\d\3(?!0000)\d{4}$/.test(object_value.trim());
  }

  // Check emails
  function checkEmails(object_value, email_cnt) {
    if (!object_value || object_value.length == 0) return true;
    var arEmails = object_value.replace(/,/g, ";").split(";");
    for (var i = 0, len = arEmails.length; i < len; i++) {
      if (email_cnt > 0 && len > email_cnt) return false;
      if (!checkEmail(arEmails[i])) return false;
    }
    return true;
  }

  // Check email
  function checkEmail(object_value) {
    if (!object_value || object_value.length == 0) return true;
    return /^[\w.%+-]+@[\w.-]+\.[A-Z]{2,18}$/i.test(object_value.trim());
  }

  // Check GUID {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
  function checkGuid(object_value) {
    if (!object_value || object_value.length == 0) return true;
    return /^(\{\w{8}-\w{4}-\w{4}-\w{4}-\w{12}\}|\w{8}-\w{4}-\w{4}-\w{4}-\w{12})$/.test(object_value.trim());
  }

  // Check URL
  function checkUrl(object_value) {
    if (!object_value || object_value.length == 0) return true;
    try {
      new URL(object_value);
    } catch (e) {
      return false;
    }
    return true;
  }

  // Check by regular expression
  function checkByRegEx(object_value, pattern) {
    if (!object_value || object_value.length == 0) return true;
    return !!object_value.match(pattern);
  }

  /**
   * Show message dialog
   *
   * @param {Event|string} arg - Event or message
   * @returns
   */
  function showMessage(arg) {
    var _arg$target;
    let doc, swal;
    try {
      // Note: If a window does not have a parent, its parent property is a reference to itself.
      [doc, swal] = [window.parent.document, window.parent.Swal];
    } catch (e) {
      // In case window.parent.document cannot be accessed
      [doc, swal] = [window.document, window.Swal];
    }
    let p = (_arg$target = arg == null ? void 0 : arg.target) != null ? _arg$target : doc,
      $div = $__default.default(p).find("div.ew-message-dialog.d-none").first(),
      msg = $div.length ? $div.text() : ""; // Text only
    if ($__default.default.isString(arg)) msg = $__default.default("<div>" + arg.trim() + "</div>").text();
    if (msg.trim() == "") return;
    if ($div.length) {
      ["success", "info", "warning", "danger"].forEach(function (type) {
        let $alert = $div.find(".alert-" + type).toggleClass("alert-" + type),
          $heading = $alert.find(".alert-heading").detach(),
          $content = $alert.children(":not(.icon)");
        $alert.find(".icon").remove();
        if ($alert[0]) {
          let w = parseInt($content.css("width"), 10); // Width specified
          if (w > 0) $content.first().css("width", "auto");
          let $toast = showToast($alert.html(), type, $heading.html());
          if (w > 0) $toast.css("max-width", w); // Override bootstrap .toast max-width
          return;
        }
      });
    }
    if ($__default.default.isString(arg)) {
      return swal.fire({
        ...ew.sweetAlertSettings,
        html: arg
      });
    }
  }

  // Random number
  function random() {
    return Math.floor(Math.random() * 100001) + 100000;
  }

  // File upload
  function upload(input) {
    let $input = $__default.default(input);
    if ($input.data("blueimpFileupload")) return;
    let id = $input.attr("name"),
      nid = id.replace(/\$/g, "\\$"),
      tbl = $input.data("table"),
      multiple = $input.is("[multiple]"),
      $dropzone = $input.closest(".ew-file-drop-zone"),
      $p = $input.closest(fieldContainerSelector),
      $ft = $p.find("#ft_" + nid),
      $fn = $p.find("#fn_" + nid),
      $fa = $p.find("#fa_" + nid),
      $label = $p.find(".ew-file-label"),
      label = $label.html();
    let _done = function (e, data) {
      if (data.result.files[0].error) return;
      let name = data.result.files[0].name,
        ar = multiple ? $fn.val() ? $fn.val().split(ew.MULTIPLE_UPLOAD_SEPARATOR) : [] : [];
      ar.push(name);
      $fn.val(ar.join(ew.MULTIPLE_UPLOAD_SEPARATOR));
      $fa.val("0");
      if (!multiple)
        // Remove other entries if not multiple upload
        $ft.find("tbody > tr:not(:last-child)").remove();
    };
    let _deleted = function (e, data) {
      let url = e.originalEvent.target.dataset.url,
        params = getSearchParams(url),
        fid = params.get("id"),
        name = params.get(fid);
      if (name) {
        let ar = $fn.val() ? $fn.val().split(ew.MULTIPLE_UPLOAD_SEPARATOR) : [],
          index = ar.indexOf(name);
        if (index > -1) ar.splice(index, 1);
        $fn.val(ar.join(ew.MULTIPLE_UPLOAD_SEPARATOR));
        $fa.val("0");
      }
    };
    let _change = function (e, data) {
      var _data$files;
      $ft.toggleClass("ew-has-rows", ((_data$files = data.files) == null ? void 0 : _data$files.length) > 0);
      let ar = $fn.val() ? $fn.val().split(ew.MULTIPLE_UPLOAD_SEPARATOR) : [];
      data.files.forEach(file => ar.push(file.name));
      let cnt = $input.data("maxNumberOfFiles");
      if ($__default.default.isNumber(cnt) && cnt > 0 && ar.length > cnt) {
        _alert(ew.language.phrase("UploadErrorMaxNumberOfFiles"));
        return false;
      }
      let l = $input.data("size");
      if ($__default.default.isNumber(l) && l > 0 && ar.join(ew.MULTIPLE_UPLOAD_SEPARATOR).length > l) {
        _alert(ew.language.phrase("UploadErrorMaxFileLength"));
        return false;
      }
    };
    let _confirmDelete = function (e) {
      if (!multiple && $fn.val()) {
        if (!confirm(ew.language.phrase("UploadOverwrite"))) {
          e.preventDefault();
          e.stopPropagation();
        }
      }
    };
    let _changed = function (e, data) {
      var _data$files2, _data$result2, _data$result2$files;
      $ft.toggleClass("ew-has-rows", ((_data$files2 = data.files) == null ? void 0 : _data$files2.length) > 0 || ((_data$result2 = data.result) == null ? void 0 : (_data$result2$files = _data$result2.files) == null ? void 0 : _data$result2$files.length) > 0);
      let ar = $fn.val() ? $fn.val().split(ew.MULTIPLE_UPLOAD_SEPARATOR) : [];
      $label.html(ar.join(", ") || label);
    };
    let _completed = function (e, data) {
      // After download template rendered
      let evt = {
        target: data.context
      };
      initLightboxes(evt);
      initPdfObjects(evt);
      ew.updateDropdownPosition();
      data.context.find("img").on("load", ew.updateDropdownPosition);
    };
    let _added = function (e, data) {
      var _data$files3;
      // After upload template rendered
      $ft.toggleClass("ew-has-rows", ((_data$files3 = data.files) == null ? void 0 : _data$files3.length) > 0);
      data.context.find(".start").click(_confirmDelete);
    };
    // let _send = function(e, data) { // Before each file upload request
    //     console.log(data);
    // };
    let uploadUrl = getApiUrl(ew.API_JQUERY_UPLOAD_ACTION),
      _downloadTemplate = $__default.default.templates("#template-download"),
      _uploadTemplate = $__default.default.templates("#template-upload"),
      form = getForm(input),
      $form = $__default.default(form),
      readonly = $form.find("#confirm").val() == "confirm" || $input.attr("readonly") || $input.prop("disabled");
    if (readonly && !$input.prop("disabled")) $input.prop("disabled", true); // Disable input button if readonly
    const {
      acceptFileTypes,
      maxFileSize,
      maxNumberOfFiles
    } = $input.data();
    const formData = {
      id: id,
      table: tbl,
      session: ew.SESSION_ID,
      replace: multiple ? "0" : "1",
      acceptFileTypes,
      maxFileSize,
      maxNumberOfFiles
    };
    $input.fileupload(ew.deepAssign({
      url: uploadUrl,
      type: "POST",
      multipart: true,
      autoUpload: true,
      loadImageFileTypes: /^image\/(gif|jpe?g|png)$/i,
      loadVideoFileTypes: /^video\/mp4$/i,
      loadAudioFileTypes: /^audio\/(mpeg|mp3)$/i,
      filesContainer: $ft,
      uploadTemplateId: null,
      downloadTemplateId: null,
      uploadTemplate: _uploadTemplate.render.bind(_uploadTemplate),
      downloadTemplate: _downloadTemplate.render.bind(_downloadTemplate),
      previewMaxWidth: ew.UPLOAD_THUMBNAIL_WIDTH,
      previewMaxHeight: ew.UPLOAD_THUMBNAIL_HEIGHT,
      dropZone: $dropzone,
      messages: {
        acceptFileTypes: ew.language.phrase("UploadErrorAcceptFileTypes"),
        maxNumberOfFiles: ew.language.phrase("UploadErrorMaxNumberOfFiles"),
        maxFileSize: ew.language.phrase("UploadErrorMaxFileSize"),
        minFileSize: ew.language.phrase("UploadErrorMinFileSize")
      },
      formData,
      readonly // Custom
    }, ew.uploadOptions))
    // .on("fileuploadsend", _send)
    .on("fileuploaddone", _done).on("fileuploaddestroy", _deleted).on("fileuploadchange", _change).on("fileuploadadded fileuploadfinished fileuploaddestroyed", _changed).on('fileuploadadded', _added).on('fileuploadcompleted', _completed);
    if ($fn.val()) {
      $__default.default.ajax({
        url: uploadUrl,
        data: {
          id: id,
          table: tbl,
          session: ew.SESSION_ID
        },
        dataType: "json",
        context: this,
        success: function (result) {
          if (result != null && result[id]) {
            let done = $input.fileupload("option", "done");
            if (done) done.call(input, $__default.default.Event(), {
              result: {
                files: result[id]
              }
            }); // Use "files"
          }

          if (readonly)
            // Hide delete button if readonly
            $ft.find("td.delete").hide();
        }
      });
    }
  }

  /**
   * Convert data to number
   *
   * @param {*} data - Data being converted
   * @returns {number}
   */
  function parseNumber(data) {
    let locale = ew.getLocaleFromPlatform(ew.LANGUAGE_ID);
    if (ew.NUMBERING_SYSTEM == "latn") locale.numeralSystem = undefined;
    if (locale.delimiters.thousands !== ew.GROUPING_SEPARATOR) locale.delimiters.thousands = ew.GROUPING_SEPARATOR;
    if (locale.delimiters.decimal !== ew.DECIMAL_SEPARATOR) locale.delimiters.decimal = ew.DECIMAL_SEPARATOR;
    return ew.parse(data, {
      locale
    });
  }

  /**
   * Get numbering system
   */
  function getNumberingSystem() {
    return ew.NUMBERING_SYSTEM || new Intl.NumberFormat(ew.LANGUAGE_ID).resolvedOptions().numberingSystem;
  }
  let DateTime = luxon__default.default.DateTime,
    Interval = luxon__default.default.Interval;

  /**
   * Format data by DateTime (see https://moment.github.io/luxon/docs/class/src/datetime.js~DateTime.html)
   *
   * @param {string|Number|Date} data - Date
   * @param {string|Array} format - Date format (see https://moment.github.io/luxon/docs/manual/formatting.html#toformat)
   * @returns {string}
   */
  function formatDateTime(data, format) {
    let dt;
    if ($__default.default.isString(data))
      // SQL dates, times, and datetimes
      dt = DateTime.fromSQL(data, {
        zone: "utc"
      }); // Do not change the time zone
    else if ($__default.default.isNumber(data))
      // Unix timestamps
      dt = DateTime.fromSeconds(data, {
        zone: "utc"
      });else if (data instanceof Date)
      // JS Date Object
      dt = DateTime.fromJSDate(data, {
        zone: "utc"
      });else if (data instanceof DateTime)
      // DateTime Object
      dt = data;
    return dt.toFormat(format, {
      locale: ew.LANGUAGE_ID,
      numberingSystem: ew.getNumberingSystem()
    });
  }

  /**
   * Format time span
   *
   * @param {Date} start - JS start date/time
   * @param {Date} end - JS end date/time
   * @param {bool} allDay - Whether event is all day
   * @returns {string}
   */
  function formatTimeSpan(start, end, allDay) {
    let from = DateTime.fromJSDate(start, {
        zone: "utc"
      }),
      to = DateTime.fromJSDate(end, {
        zone: "utc"
      });
    if (to.isValid && to.equals(to.startOf("day")))
      // 12:00AM
      to = to.minus(1); // Minus one millisecond => back to previous day
    let interval = new Interval({
      start: from,
      end: to
    });
    if (interval.isValid) {
      // Check if all day
      let days = interval.count() / (24 * 60 * 60 * 1000);
      allDay || (allDay = Number.isInteger(days) && days >= 1);
    }
    if (allDay) {
      // All day => Show date only
      if (interval.isValid && interval.count("days") > 1 || from.isValid && to.isValid && from.startOf("day") < to.startOf("day")) return formatDateTime(from, ew.DATE_FORMAT) + " - " + formatDateTime(to, ew.DATE_FORMAT);else if (from.isValid && !end)
        // No end date
        return formatDateTime(from, ew.DATE_FORMAT);
      return "";
    } else {
      // Not all day => Show date/time
      if (from.isValid && to.isValid) return from.startOf("day") < to.startOf("day") ? formatDateTime(from, ew.DATE_FORMAT + " " + ew.TIME_FORMAT) + " - " + formatDateTime(to, ew.DATE_FORMAT + " " + ew.TIME_FORMAT) : formatDateTime(from, ew.TIME_FORMAT) + " - " + formatDateTime(to, ew.TIME_FORMAT);else if (from.isValid) return formatDateTime(from, ew.TIME_FORMAT);
    }
    return "";
  }

  /**
   * Parse data to DateTime (see https://moment.github.io/luxon/api-docs/index.html#datetime)
   *
   * @param {string} data - Date/Time string supported by DateTime
   * @param {string|Array} format - Date format (see https://moment.github.io/luxon/#/formatting?id=table-of-tokens)
   * @returns {DateTime}
   */
  function parseDateTime(data, format) {
    return DateTime.fromFormat(data, format, {
      locale: ew.LANGUAGE_ID,
      numberingSystem: ew.getNumberingSystem()
    });
  }

  /**
   * Check if data can be parsed to DateTime (see https://moment.github.io/luxon/api-docs/index.html#datetime)
   *
   * @param {string} data - Date string supported by DateTime
   * @param {string|Array} format - Date format (see https://moment.github.io/luxon/#/formatting?id=table-of-tokens)
   * @returns {boolean}
   */
  function checkDate(data, format) {
    if (!data || data.length == 0) return true;
    return parseDateTime(data, format).isValid;
  }

  // Check time (alias of checkDate)
  function checkTime(data, format) {
    if (!data || data.length == 0) return true;
    return parseDateTime(data, format).isValid;
  }

  /**
   * Format currency
   *
   * @param {number} value - Value
   * @param {string} format - Formatter pattern
   */
  function formatCurrency(value, format) {
    format || (format = ew.CURRENCY_FORMAT);
    if (format.includes(";")) {
      let formats = format.split(";");
      format = value >= 0 ? formats[0] : formats[1];
    }
    format = format.replace("¤", "$");
    let locale = ew.getLocaleFromPlatform(ew.LANGUAGE_ID);
    if (ew.NUMBERING_SYSTEM == "latn") locale.numeralSystem = undefined;
    if (locale.delimiters.thousands !== ew.GROUPING_SEPARATOR) locale.delimiters.thousands = ew.GROUPING_SEPARATOR;
    if (locale.delimiters.decimal !== ew.DECIMAL_SEPARATOR) locale.delimiters.decimal = ew.DECIMAL_SEPARATOR;
    return ew.format(value, format, {
      locale,
      currency: (ew.IS_RTL ? "\u200E" : "") + ew.CURRENCY_SYMBOL
    }); // Make sure the currency symbol position is not moved.
  }

  /**
   * Format number
   *
   * @param {number} value - Value
   * @param {string} format - Formatter pattern
   */
  function formatNumber(value, format) {
    let locale = ew.getLocaleFromPlatform(ew.LANGUAGE_ID);
    if (ew.NUMBERING_SYSTEM == "latn") locale.numeralSystem = undefined;
    if (locale.delimiters.thousands !== ew.GROUPING_SEPARATOR) locale.delimiters.thousands = ew.GROUPING_SEPARATOR;
    if (locale.delimiters.decimal !== ew.DECIMAL_SEPARATOR) locale.delimiters.decimal = ew.DECIMAL_SEPARATOR;
    return ew.format(value, format || ew.NUMBER_FORMAT, {
      locale
    });
  }

  /**
   * Format percent
   *
   * @param {number} value - Value
   * @param {string} format - Formatter pattern
   */
  function formatPercent(value, format) {
    let locale = ew.getLocaleFromPlatform(ew.LANGUAGE_ID);
    if (ew.NUMBERING_SYSTEM == "latn") locale.numeralSystem = undefined;
    if (locale.delimiters.thousands !== ew.GROUPING_SEPARATOR) locale.delimiters.thousands = ew.GROUPING_SEPARATOR;
    if (locale.delimiters.decimal !== ew.DECIMAL_SEPARATOR) locale.delimiters.decimal = ew.DECIMAL_SEPARATOR;
    return ew.format(value, format || ew.PERCENT_FORMAT, {
      locale
    });
  }

  /**
   * Init page
   *
   * @param {Event|undefined} e - Event
   */
  function initPage(e) {
    var _e$target12;
    var el = (_e$target12 = e == null ? void 0 : e.target) != null ? _e$target12 : document,
      $el = $__default.default(el),
      $tables = $el.find("table.ew-table:not(.ew-export-table)");
    ew.initPanels(el); // Init grid panels
    ew.renderJsTemplates(e);
    lazyLoad(e);
    initForms(e);
    initTooltips(e);
    initPasswordOptions(e);
    initIcons(e);
    initSearchFilters(e);
    initLightboxes(e);
    initPdfObjects(e);
    $tables.each(setupTable); // Init tables
    $el.find(".ew-column-dropdown").each(function () {
      var _localStorage$getItem;
      let table = this.dataset.table;
      (_localStorage$getItem = localStorage.getItem(ew.PROJECT_NAME + "_" + table + "_invisible_fields")) == null ? void 0 : _localStorage$getItem.split(",").forEach(field => $__default.default("#tbl_" + table + "list").find("th[data-name='" + field + "'],td[data-name='" + field + "']").toggleClass("d-none", true));
      $__default.default(this).find(".ew-dropdown-checkbox").on("click", function (e) {
        let input = this.querySelector(".ew-dropdown-check-input[data-field]"),
          field = input == null ? void 0 : input.dataset.field;
        if (table && field) {
          input.classList.toggle("ew-checked");
          $__default.default("#tbl_" + table + "list").find("th[data-name='" + field + "'],td[data-name='" + field + "']").toggleClass("d-none", !input.classList.contains("ew-checked"));
        }
        localStorage.setItem(ew.PROJECT_NAME + "_" + table + "_invisible_fields", Array.from(e.currentTarget.closest(".dropdown-menu").querySelectorAll(".ew-dropdown-check-input[data-field]:not(.ew-checked)"), el => el.dataset.field));
      });
    }).on("show.bs.dropdown", function (e) {
      let table = e.currentTarget.dataset.table,
        inputs = e.currentTarget.querySelectorAll(".ew-dropdown-check-input[data-field]");
      for (let input of inputs) {
        let field = input.dataset.field;
        input.classList.toggle("ew-checked", !!$__default.default("#tbl_" + table + "list").find("th[data-name='" + field + "']:not(.d-none)")[0]);
      }
    });
    initExportLinks(e);
    initMultiSelectCheckboxes(e);

    // Report
    var $rpt = $el.find(".ew-report");
    if ($rpt[0]) {
      $rpt.find(".card").on("collapsed.lte.widget", function () {
        // Fix min-height when .lte.widget is collapsed
        var $card = $__default.default(this),
          $div = $card.closest("[class^='col-']"),
          mh = $div.css("min-height");
        if (mh) $div.data("min-height", mh);
        $div.css("min-height", 0);
      }).on("expanded.lte.widget", function () {
        // Fix min-height when .lte.widget is expanded
        var $card = $__default.default(this),
          $div = $card.closest("[class^='col-']"),
          mh = $div.css("min-height");
        if (mh) $div.css("min-height", mh); // Restore min-height
      });
      // Group expand/collapse button
      $rpt.find(".ew-group-toggle").on("click", function () {
        ew.toggleGroup(this);
      });
    }

    // Show message
    if (typeof ew.USE_JAVASCRIPT_MESSAGE != "undefined" && ew.USE_JAVASCRIPT_MESSAGE) showMessage(e);
  }

  // Redirect by HTTP GET or POST
  function redirect(url, f, method, data) {
    var _ew$vars;
    let urls = (_ew$vars = ew.vars) != null && _ew$vars.login ? Array.from(Object.entries(ew.vars.login)).filter(entry => entry[0].endsWith("Url")).map(entry => entry[1]) : [];
    if (urls.includes(url)) {
      // Known URLs
      window.location = url;
      return false;
    }
    let newUrl;
    if (url.startsWith("http")) {
      newUrl = new URL(url);
    } else if (url.startsWith("/")) {
      newUrl = new URL(url, location.protocol + "//" + location.host);
    } else {
      _alert(ew.language.phrase("IncorrectUrl"));
      return false;
    }
    let params = newUrl.searchParams;
    if (data) params = mergeSearchParams(params, data);
    if (sameText(method, "post")) {
      // POST
      const form = f || document.createElement("form");
      form.method = "post";
      form.action = url.split("?")[0];
      document.body.appendChild(form);
      params.set(ew.TOKEN_NAME_KEY, ew.TOKEN_NAME);
      params.set(ew.ANTIFORGERY_TOKEN_KEY, ew.ANTIFORGERY_TOKEN);
      params.forEach((value, key) => {
        const el = document.createElement("input");
        el.type = "hidden";
        el.name = key;
        el.value = ew.sanitize(value);
        form.appendChild(el);
      });
      form.submit();
    } else {
      // GET
      window.location = sanitizeUrl(newUrl.toString());
    }
    return false;
  }

  // Show/Hide password
  function togglePassword(e) {
    let $btn = $__default.default(e.currentTarget),
      $input = $btn.closest(".input-group").find("input"),
      $i = $btn.find("i");
    if ($input.attr("type") == "text") {
      $input.attr("type", "password");
      $i.toggleClass("fa-eye-slash fa-eye");
    } else if ($input.attr("type") == "password") {
      $input.attr("type", "text");
      $i.toggleClass("fa-eye-slash fa-eye");
    }
  }

  // Re-order chart
  function updateChart(el) {
    let url = new URL(el.form.action),
      $container = $__default.default(el).closest(".ew-chart-top, .ew-chart-bottom"),
      $overlay = $__default.default(ew.overlayTemplate());
    url.search = $__default.default(el.form).serialize();
    $container.addClass("ew-loading").append($overlay);
    fetch(url.toString()).then(async response => {
      let $html = $__default.default("<div>" + (await response.text()) + "</div>"),
        $new = $html.find(".ew-chart-top, .ew-chart-bottom").filter("[data-chart='" + $container.data("chart") + "']"),
        $debug = $html.find(".ew-debug");
      if ($new[0]) $container.replaceWith($new);
      if ($debug[0]) $new.next(".ew-debug").replaceWith($debug);
    }).finally(() => $container.removeClass("ew-loading").find($overlay).remove());
  }

  // Check if larger than mediumn screen width (not mobile)
  function isMediumScreen() {
    return window.matchMedia(ew.screenMediaQuery).matches;
  }

  // Check if mobile
  function isMobile() {
    return ew.IS_MOBILE || !isMediumScreen();
  }

  /**
   * Refresh page
   *
   * @param {Promise} promise Promise returned by fetch()
   * @param {string} context CSS class name of context
   * @returns {Promise}
   */
  function refresh(promise, context) {
    var _context;
    (_context = context) != null ? _context : context = ".ew-card.ew-grid";
    context = context.startsWith(".") || context.startsWith("#") ? context : "." + context;
    let target = document.querySelector(context + ", .ew-multi-column-grid, .ew-view-form, .ew-edit-form, main.ew-no-record");
    if (!target) return;
    let $overlay = $__default.default(ew.overlayTemplate()),
      $target = $__default.default(target).addClass("ew-loading").toggleClass("overlay-wrapper", !target.classList.contains("card")).append($overlay);
    return promise.then(async response => {
      if (!(response instanceof Response)) return response;
      let ct = response.headers.get("Content-Type");
      // Handle JSON
      if (ct != null && ct.includes("json")) {
        let result = await response.json(),
          validation = result.validation,
          error = getError(result),
          form = target.querySelector(".ew-form"),
          f = ew.forms.get(form);
        if (f && validation) {
          // Validation errors
          for (let [key, value] of Object.entries(validation)) {
            key = parseInt(key, 10);
            if ($__default.default.isNumber(key) && $__default.default.isObject(value)) {
              form.dataset.rowindex = key;
              for (let [fldvar, msg] of Object.entries(value)) f.addCustomError(fldvar, msg, key);
            } else if ($__default.default.isString(value)) {
              f.addCustomError(key, value);
            }
          }
          f.focus();
        }
        if (error) showToast(error);else if ($__default.default.isString(result.warningMessage)) showToast(result.warningMessage, "warning");else if ($__default.default.isString(result.message)) showToast(result.message, "body");else if ($__default.default.isString(result.successMessage)) showToast(result.successMessage, "success");
        return false; // Return false
      }
      // Handle HTML
      let html = await response.text(),
        url = new URL(response.url),
        selectors = [".ew-message-dialog", ".ew-chart-top", ".ew-chart-bottom", ".ew-debug .card-body", ".ew-timer"];
      if (url.searchParams.get("layout") == "false") {
        html = "<div>" + html + "</div>";
        url.searchParams.delete("layout");
      }
      url.searchParams.delete("rnd");
      url.searchParams.delete("action");
      html = getContent(html, true); // Keep scripts
      let $html = $__default.default("<div>" + html + "</div>"); // New HTML
      if (!target.closest(".ew-dashboard")) {
        // Not dashboard
        selectors.unshift(".ew-toolbar"); // Update toolbar also
        if (samePath(url, currentUrl) && currentUrl.toString() != url.toString())
          // Only update browser URL if same page or view/edit page
          window.history.pushState(null, "", url.toString()); // Update browser URL
      }

      if ($html.find("main.ew-no-record")[0] || $target.is("main.ew-no-record")) {
        // New or old HTML contains no records => update <main> element
        selectors = [".ew-ext-search-form", "main", ...selectors];
      } else if ($target.hasClass("ew-view-form")) {
        // View page
        selectors = [".ew-view-form", ".ew-pager", ...selectors];
      } else if ($target.hasClass("ew-edit-form")) {
        // Edit page
        selectors = [".ew-edit-form", ".ew-pager", ...selectors];
      } else if ($target.hasClass("ew-multi-column-grid")) {
        // Multi-column page
        selectors = [".ew-ext-search-form", ".ew-multi-column-grid", ...selectors];
      } else {
        // List/Report page
        selectors = ["#ew-filter-list", ".ew-ext-search-form", context + " .card-header", context + " .card-body", context + " .card-footer", ...selectors];
      }
      selectors.forEach(selector => $__default.default(selector).each((i, el) => {
        let $old = $__default.default(el),
          $new = $html.find(selector).eq(i);
        if ($old[0] && $new[0]) {
          delete el.dataset.isset; // Reset
          $old.html($new.html()); // Replace inner HTML
          $old.attr("class", $new.attr("class")); // Update class name
        }

        if (selector == ".ew-toolbar") {
          let e = {
            target: el
          };
          initIcons(e);
          initSearchFilters(e);
        } else if (selector == ".ew-ext-search-form") {
          ew.initForms({
            target: $new[0]
          });
          let $tmpl = $new.find("#navbar-basic-search");
          if ($tmpl[0]) {
            let html = $tmpl.render(null, ew.jsRenderHelpers),
              $html = $__default.default(html),
              $search = $html.find("#" + ew.TABLE_BASIC_SEARCH),
              $searchType = $html.find("#" + ew.TABLE_BASIC_SEARCH_TYPE);
            if ($search.val() != $__default.default("#" + ew.TABLE_BASIC_SEARCH).val() || $searchType.val() != $__default.default("#" + ew.TABLE_BASIC_SEARCH_TYPE).val()) $__default.default(".nav-item.navbar-basic-search").replaceWith($html);
          }
          $old.find(".ew-search-operator .form-select.ew-operator-select").each((i, el) => searchOperatorChange(el));
        } else if (selector == ".ew-message-dialog") {
          if (!$new.hasClass("d-none")) $old.html($new.html()); // Replace inline html
        }
      }));

      selectors.filter(selector => selector != ".ew-message-dialog").forEach(selector => $html.find(selector).remove()); // Remove HTML used (except message)
      removeScript("refresh"); // Remove old scripts
      executeScript($html.html(), "refresh"); // Execute scripts, e.g. custom template
      let e = $__default.default.Event({
        type: "load.ew",
        target
      });
      ew.initPage(e);
      if (selectors.includes(".ew-toolbar")) ew.initExportLinks(); // Export links are not inside e.target
      target.querySelectorAll(".btn-group").forEach(el => {
        var _bootstrap$Tooltip$ge7;
        return (_bootstrap$Tooltip$ge7 = bootstrap.Tooltip.getInstance(el)) == null ? void 0 : _bootstrap$Tooltip$ge7.update();
      }); // Update tooltip position or it may obscure other buttons
      $__default.default(".toast.show").toast("dispose"); // Hide toasts
      showMessage({
        target: $html
      }); // Show message, if any
      $document$1.trigger(e);
      $document$1.trigger($__default.default.Event({
        type: "refresh.ew",
        target
      }));
    }).finally(() => $target.removeClass("ew-loading").find($overlay).remove());
  }

  // Check if dark mode
  function isDark() {
    return document.body.classList.contains("dark-mode");
  }

  // Layout
  var _fixLayoutHeightTimer;

  // Fix layout height
  function fixLayoutHeight() {
    if (_fixLayoutHeightTimer) _fixLayoutHeightTimer.cancel(); // Clear timer
    _fixLayoutHeightTimer = $__default.default.later(50, null, function () {
      var layout = $body.data("lte.layout");
      if (layout) layout.fixLayoutHeight();
    });
  }

  // Add user event handlers
  function addEventHandlers(tblVar) {
    let fields = ew.events[tblVar];
    if (fields) {
      for (const [fldVar, events] of Object.entries(fields)) $__default.default('[data-table=' + tblVar + '][data-field=' + fldVar + ']').on(events);
    }
  }

  var functions = {
    __proto__: null,
    currentUrl,
    forms,
    AjaxLookup,
    AutoSuggest,
    Form,
    Field,
    Select2Utils: Utils,
    Select2Defaults: Defaults,
    fieldContainerSelector,
    createSelect,
    createModalLookup,
    createFilter,
    initIcons,
    initPasswordOptions,
    getApiUrl,
    parseUrl,
    createSearchParams,
    getSearchParams,
    getSearchParam,
    setSearchParams,
    mergeSearchParams,
    setSearchParam,
    deleteSearchParam,
    buildUrl,
    setLayout,
    sanitizeUrl,
    setSessionTimer,
    initExportLinks,
    initMultiSelectCheckboxes,
    getError,
    fileDownload,
    lazyLoad,
    updateDropdownPosition,
    initLightboxes,
    initPdfObjects,
    initTooltips,
    parseJson,
    searchOperatorChange,
    initForms,
    isFunction: isFunction$2,
    alert: _alert,
    prompt: _prompt,
    toast,
    showToast,
    getForm,
    hasFormData,
    setSearchType,
    updateOptions,
    getUserParams,
    getQueryBuilderFilterInput,
    getQueryBuilderValueSetter,
    getQueryBuilderFilterValidation,
    applyTemplate,
    exportCustom,
    toggleGroup,
    convertToBool,
    valueChanged,
    setLanguage,
    submitAction,
    getchartParams,
    _export,
    export: _export,
    removeSpaces,
    isHiddenTextArea,
    isModalLookup,
    isFilter,
    isAutoSuggest,
    isSelect2,
    isNativeSelectOne,
    isTextbox,
    isBooleanCheckbox,
    clearError,
    onError,
    setFocus,
    setInvalid,
    setValid,
    hasValue,
    isMaskedPassword,
    sort,
    filter,
    scrollIntoView,
    inlineAction,
    confirmDelete,
    keySelected,
    selectAllKeys,
    selectAll,
    updateSelected,
    clearSelected,
    clearDelete,
    clickDelete,
    selectKey,
    setupTable,
    setupGrid,
    addGridRow,
    deleteGridRow,
    htmlEncode,
    htmlDecode,
    getElements,
    getElement,
    getAncestorBy,
    isHidden,
    sameText,
    sameString,
    headerCase,
    samePath,
    getValue,
    getOptionValues,
    getOptionValue,
    getOptionTexts,
    clearOptions,
    getId,
    valueSeparator,
    displayValue,
    optionHtml,
    optionsHtml,
    newOption,
    selectOption,
    fetch: _fetch,
    executeScript,
    stripScript,
    addScript,
    removeScript,
    getContent,
    getOptions,
    enable2FA,
    disable2FA,
    sendOTP,
    showBackupCodes,
    addOptionDialogShow,
    modalDialogHide,
    modalDialogShow,
    importDialogShow,
    autoFill,
    tooltip,
    initSearchFilters,
    emailDialogShow,
    exportEmail,
    showDrillDown,
    ajax,
    currentPage,
    toggleSearchOperator,
    toggleLayout,
    copyToClipboard,
    checkInteger,
    checkNumber,
    escapeRegExChars,
    checkRange,
    checkPhone,
    checkZip,
    checkCreditCard,
    checkSsn,
    checkEmails,
    checkEmail,
    checkGuid,
    checkUrl,
    checkByRegEx,
    showMessage,
    random,
    upload,
    parseNumber,
    getNumberingSystem,
    formatDateTime,
    formatTimeSpan,
    parseDateTime,
    parseDate: parseDateTime,
    parseTime: parseDateTime,
    checkDate,
    checkTime,
    formatCurrency,
    formatNumber,
    formatPercent,
    initPage,
    redirect,
    togglePassword,
    updateChart,
    isMediumScreen,
    isMobile,
    refresh,
    isDark,
    fixLayoutHeight,
    addEventHandlers
  };

  /**
   * Create FullCalendar object
   *
   * @param {HTMLElement} el - HTML element
   * @param {Object} options - Calendar options
   * @param {Object} options.fullCalendarOptions - FullCalendar options
   * @param {Object} options.eventPopoverOptions - Event popover options
   * @param {HTMLElement} dropdown - HTML element
   * @returns calendar object
   */
  function fullCalendar(el, options, dropdown) {
    let {
        updateTable,
        eventFields,
        addUrl,
        viewUrl,
        editUrl,
        deleteUrl,
        copyUrl,
        fullCalendarOptions,
        eventPopoverOptions
      } = options,
      calendar;

    // Remove addUrl in options
    delete options.addUrl;

    // Update event from server side JSON response
    let processJson = data => {
      //console.log(result);
      let convertValue = (key, value) => {
        let match;
        if (typeof value == "string" && (match = value.match(/^(\d{4}-\d{2}-\d{2}) (\d{2}:\d{2}:\d{2})/)))
          // Make sure ISO date
          return match[1] + "T" + match[2];
        if (key == "allDay")
          // Make sure Boolean
          return ew.convertToBool(value);else if (key == "url" && !value) return ""; // Make sure not null
        else if (key == "display" && !value) return "auto"; // Make sure not null
        else if (key == "classNames" && value && $__default.default.isString(value))
          // Make sure return as array
          return value.replace(/\s+/g, " ").split(" ");else if (key == "classNames" && !value)
          // Make sure return as array
          return [""];else if (key == "groupId" && !value)
          // Make sure return as stringy
          return "";else if (key == "description" && !value) return ""; // Make sure not null
        else return value;
      };
      if (data.success) {
        let action = data.action,
          events = data[updateTable];
        if (!Array.isArray(events)) events = [events];
        for (let event of events) {
          for (let [key, name] of Object.entries(eventFields)) {
            // Normalize keys in event
            if (key != name && name !== null && name in event) {
              event[key] = event[name];
              delete event[name];
            }
          }
          if (action == ew.API_DELETE_ACTION) {
            var _calendar$getEventByI;
            // Remove deleted event
            (_calendar$getEventByI = calendar.getEventById(event.id)) == null ? void 0 : _calendar$getEventByI.remove();
          } else if (action == ew.API_ADD_ACTION) {
            // Add/Copy event
            for (let name in event) {
              // Check new properties
              let key = Object.keys(eventFields).find(key => key === name),
                value = event[name];
              if (key && value !== undefined)
                // Value can be null
                event[key] = convertValue(key, value); // Convert value
            }

            calendar.addEvent(event);
          } else {
            // Update event
            let curEvent = calendar.getEventById(event.id);
            let groupId = event.groupId;
            for (let name in event) {
              // Update properties
              let key = Object.keys(eventFields).find(key => key === name),
                value = convertValue(key, event[name]);
              if (key && value !== undefined) {
                // Value can be null
                value = convertValue(key, value); // Convert value
                if (key == "start") curEvent.setStart(value);else if (key == "end") curEvent.setEnd(value);else if (key == "allDay") curEvent.setAllDay(value);else if (key == "description") curEvent.setExtendedProp(key, value);else if (key != "id" && !groupId)
                  // setProp will change all related events, https://fullcalendar.io/docs/Event-setProp
                  curEvent.setProp(key, value);
              } else if (value) {
                curEvent.setExtendedProp(name, value);
              }
            }
          }
        }
      }
    };

    // Process event
    let processEvent = info => {
      let event = info.event,
        events = [event, ...info.relatedEvents],
        apiAction = event.id ? ew.API_EDIT_ACTION : ew.API_ADD_ACTION;
      events.forEach(event => processRequest(event, apiAction).then(data => {
        if (!data.success) {
          // Revert if not success
          info.revert();
        } else {
          // Update event
          processJson(data);
        }
      }).catch(err => {
        // Revert if error
        console.log(err);
        info.revert();
      }));
    };

    // Process request
    let processRequest = (event, apiAction) => new Promise((resolve, reject) => {
      // Set up post data
      let obj = event.toPlainObject({
          collapseExtendedProps: true
        }),
        data = {};
      // Event fields
      for (let [key, name] of Object.entries(eventFields)) {
        if (key in obj) {
          let val = obj[key];
          name || (name = key);
          if (val instanceof Date)
            // Handle date field, need to convert to ISO string
            val = val.toISOString().slice(0, -5).replace(/T/, " "); // Assume UTC
          else if (typeof val == "string" && val.match(/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}/)) val = val.substring(0, 19).replace(/T/, " ");else if (typeof val === "boolean")
            // Handle boolean field
            val = val ? "1" : "0";else if (key == "classNames") val = Array.isArray(val) ? val.join(" ") : val; // Convert array to string
          data[name] = val;
          delete obj[key];
        }
      }
      data = {
        ...data,
        ...obj
      };
      // Post to Event Table add/edit/delete API
      let apiUrl = apiAction == ew.API_ADD_ACTION ? ew.getApiUrl([apiAction, updateTable]) : ew.getApiUrl([apiAction, updateTable, event.id]);
      $__default.default.post(apiUrl, data, data => resolve(data)).fail(err => reject(err));
    });

    // Get field parameter
    let getFldParm = name => Object.keys(currentTable.fields).find(key => currentTable.fields[key].name == name);

    // Add menu ID to URL
    let urlAddId = (url, id) => {
      let ar = url.split("?");
      ar[0] += id ? (!ar[0].endsWith("/") ? "/" : "") + id : "";
      return ar.join("?");
    };

    // Add/Copy event
    let addEventDialogShow = data => {
      let url = addUrl,
        params = new URLSearchParams();
      if ($__default.default.isObject(data) && (data.startStr || data.endStr /* || data.allDay */)) {
        let startFld = getFldParm(eventFields["start"]),
          endFld = getFldParm(eventFields["end"]) /*,
                                                  allDayFld = getFldParm(eventFields["allDay"])*/;
        if (startFld && data.startStr) params.set(startFld, data.startStr);
        if (endFld && data.endStr) params.set(endFld, data.endStr);
        // if (allDayFld && data.allDay)
        //     params.set(allDayFld, "1");
      }

      if ($__default.default.isString(data) && data.match(/^\d+$/))
        // Copy
        url = urlAddId(url, data);
      let qs = params.toString();
      url += qs ? "?" + qs : "";
      ew.modalDialogShow({
        url: url,
        callback: processJson,
        json: true,
        btn: ew.language.phrase("AddBtn"),
        caption: ew.language.phrase("AddLink")
      });
    };

    // View event
    let viewEventDialogShow = id => ew.modalDialogShow({
      url: urlAddId(viewUrl, id),
      callback: processJson,
      json: true,
      btn: null,
      caption: ew.language.phrase("ViewLink")
    });

    // Edit event
    let editEventDialogShow = id => ew.modalDialogShow({
      url: urlAddId(editUrl, id),
      callback: processJson,
      json: true,
      btn: null,
      caption: ew.language.phrase("EditLink")
    });

    // Delete event
    let deleteEvent = id => ew.prompt(ew.language.phrase("DeleteEventConfirm"), result => {
      if (result) ew.modalDialogShow({
        url: ew.setSearchParams(urlAddId(deleteUrl, id), {
          action: "delete"
        }),
        callback: processJson,
        json: true
      });
    });

    // Add custom add button
    if (addUrl) {
      fullCalendarOptions.customButtons = {
        add: {
          text: ew.language.phrase("AddEvent"),
          click: addEventDialogShow
        }
      };
      fullCalendarOptions.headerToolbar.left += " add";
    }

    // Get or create popover
    let getOrCreatePopover = info => {
      let config = {
        ...bootstrap.Popover.Default,
        ...eventPopoverOptions,
        trigger: "manual" // Set trigger to "manual"
      };

      let popover = bootstrap.Popover.getOrCreateInstance(info.el, config),
        template = Array.from(document.querySelectorAll("script[data-name='event-popover']")).sort((a, b) => {
          a = parseInt(a.dataset.seq, 10) || 0;
          b = parseInt(b.dataset.seq, 10) || 0;
          return a - b;
        }).shift(),
        tmpl = $__default.default.templates(template.text);
      info.event.setExtendedProp("timeSpan", ew.formatTimeSpan(info.event.start, info.event.end, info.event.allDay)); // Add extended property "timeSpan"
      let content = tmpl.render(info.event, ew.jsRenderHelpers).trim();
      if (content) {
        var _Array$from$find$repl, _Array$from$find;
        let popoverClass = (_Array$from$find$repl = (_Array$from$find = Array.from(info.el.classList.values()).find(name => name.match(/^event-([\w+]+)\b/))) == null ? void 0 : _Array$from$find.replace(/^event-/, "popover-")) != null ? _Array$from$find$repl : "popover-default";
        popover._templateFactory = null; // Make sure popover create tip again from _config
        popover._config.title = info.event.title;
        popover._config.content = content;
        popover._config.customClass += (popover._config.customClass ? " " : "") + "ew-event-popover " + popoverClass;
        return popover;
      }
      return null;
    };

    // Context menu
    let generateGetBoundingClientRect = info => {
        let x = 0,
          y = 0;
        if (info) {
          let rect = info.el.getBoundingClientRect(),
            evt = info.jsEvent;
          x = rect.left + evt.offsetX, y = rect.top + evt.offsetY;
        }
        return () => ({
          width: 0,
          height: 0,
          top: y,
          right: x,
          bottom: y,
          left: x
        });
      },
      virtualElement = {
        getBoundingClientRect: generateGetBoundingClientRect()
      },
      bsDropdown;
    if (dropdown && options.useContextMenu && (viewUrl || editUrl || addUrl && copyUrl || deleteUrl)) {
      dropdown.dataset.bsToggle = "dropdown"; // For clearing dropdown menu
      document.addEventListener("keydown", event => event.key !== "Escape" || bootstrap.Dropdown.clearMenus());
      bsDropdown = bootstrap.Dropdown.getOrCreateInstance(dropdown, {
        reference: virtualElement
      });
      bsDropdown._menu.querySelectorAll(".dropdown-item").forEach(item => item.addEventListener("click", e => {
        let dataset = e.currentTarget.dataset,
          action = dataset.action,
          id = dataset.eventId;
        if (action && id) {
          if (action == "view" && viewUrl) viewEventDialogShow(id);else if (action == "edit" && editUrl) editEventDialogShow(id);else if (action == "copy" && addUrl && copyUrl)
            // Copy
            addEventDialogShow(id);else if (action == "delete" && deleteUrl) deleteEvent(id);
        }
      }));
    }

    // Options
    let opts = ew.deepAssign(fullCalendarOptions, {
      // events
      //eventDragStart: info => console.log(info),
      //eventDragStop: info => console.log(info),
      drop: function (info) {
        // Remove element
        let checkbox = document.getElementById("remove-event");
        if (checkbox.checked) info.draggedEl.parentNode.removeChild(info.draggedEl);
      },
      select: event => addUrl && addEventDialogShow(event),
      eventReceive: processEvent,
      // Event created
      eventDragStart: info => {
        var _bootstrap$Popover$ge;
        return (_bootstrap$Popover$ge = bootstrap.Popover.getInstance(info.el)) == null ? void 0 : _bootstrap$Popover$ge.hide();
      },
      eventDrop: processEvent,
      // Event changed (Drag / Drop)
      eventResize: processEvent,
      // Event changed (Resize)
      eventClick: info => {
        // Event click
        info.jsEvent.preventDefault();
        if (info.jsEvent.button == 2) {
          // Right click
          if (bsDropdown) {
            var _bootstrap$Popover$ge2;
            (_bootstrap$Popover$ge2 = bootstrap.Popover.getInstance(info.el)) == null ? void 0 : _bootstrap$Popover$ge2.hide();
            bsDropdown.hide();
            bsDropdown._menu.querySelectorAll(".dropdown-item").forEach(item => {
              item.dataset.eventId = info.event.id;
              item.classList.toggle("d-none", !options[item.dataset.action + "Url"]);
            });
            if (bsDropdown._menu.querySelectorAll(".dropdown-item:not(.d-done)").length) {
              virtualElement.getBoundingClientRect = generateGetBoundingClientRect(info);
              bsDropdown.show();
            }
          }
        } else if (info.event.url) {
          window.open(info.event.url);
        } else if (options.usePopover && eventPopoverOptions.trigger.match(/\bclick\b/)) {
          var _getOrCreatePopover;
          (_getOrCreatePopover = getOrCreatePopover(info)) == null ? void 0 : _getOrCreatePopover.toggle();
        } else if (options.showViewPageOnEventClick) {
          viewEventDialogShow(info.event.id);
        }
      },
      eventMouseEnter: info => {
        var _bsDropdown, _getOrCreatePopover2;
        if ((_bsDropdown = bsDropdown) != null && _bsDropdown._isShown(bsDropdown._menu) /* && bsDropdown._element.dataset.eventId == info.event.id*/) return;
        if (info.view.type == "dayGridMonth" && options.usePopover && eventPopoverOptions.trigger.match(/\bhover\b/)) (_getOrCreatePopover2 = getOrCreatePopover(info)) == null ? void 0 : _getOrCreatePopover2.show();
      },
      eventMouseLeave: info => {
        var _bsDropdown2, _bootstrap$Popover$ge3;
        if ((_bsDropdown2 = bsDropdown) != null && _bsDropdown2._isShown(bsDropdown._menu) /* && bsDropdown._element.dataset.eventId == info.event.id*/) return;
        if (info.view.type == "dayGridMonth" && options.usePopover && eventPopoverOptions.trigger.match(/\bhover\b/)) (_bootstrap$Popover$ge3 = bootstrap.Popover.getInstance(info.el)) == null ? void 0 : _bootstrap$Popover$ge3.hide();
      }
    });

    // Trigger "calendar" event
    let args = {
      element: el,
      options: opts
    };
    $__default.default(document).trigger("calendar", [args]);

    // Create calendar
    calendar = new window.FullCalendar.Calendar(el, args.options);

    // Return calendar
    return calendar;
  }

  var fullCalendar$1 = {
    __proto__: null,
    fullCalendar
  };

  /**
   * Service worker
   */
  ew__default.default.SERVICE_WORKER = "sw.js";

  /**
   * Show dialog for push notification
   *
   * @param {Object} args - Arguments
   * @param {string} args.hdr - Dialog header
   * @param {string} args.url - URL of web push API
   * @param {MouseEvent} args.evt - Mouse event
   * @returns false
   */
  async function pushNotificationDialogShow(args) {
    let $dlg = ew__default.default.pushDialog || $__default.default("#ew-push-notification-dialog").on("shown.bs.modal", e => setTimeout(() => {
      var _e$target$querySelect;
      return (_e$target$querySelect = e.target.querySelector(".modal-body .form-control")) == null ? void 0 : _e$target$querySelect.focus();
    }, 200)).on("click", ".modal-footer .btn-primary", function (e) {
      var _$$closest$find$data;
      e.preventDefault();
      if ((_$$closest$find$data = $__default.default(this).closest(".modal").find(".modal-body form").data("form")) != null && _$$closest$find$data.submit()) $dlg.modal("hide");
    });
    if (!$dlg[0]) {
      console.log("DIV #ew-push-notificatoin-dialog not found");
      return false;
    }
    if (!args.url) {
      // No API URL
      console.log("Missing URL of Web Push API");
      return false;
    }
    let target = args.evt.currentTarget,
      // Button
      $form = $__default.default(target.form),
      $f = $dlg.find(".modal-body form"),
      frm = $f.data("form");
    $f.data("all", false); // Reset
    if (!$form.find("input[name='key_m[]']:checked")[0]) {
      // No keys selected
      let result = await ew__default.default.prompt(ew__default.default.language.phrase("SendPushNotificationsToAll"), bool => bool); // Callback returns result as boolean
      $f.data("all", result);
      if (!result)
        // Cancelled
        return;
    }
    if (!frm) {
      frm = new ew__default.default.FormBuilder().setId($f.attr("id")).addFields([["title", ew__default.default.Validators.required(ew__default.default.language.phrase("PushNotificationFormTitle"))], ["body", ew__default.default.Validators.required(ew__default.default.language.phrase("PushNotificationFormBody"))]]).setValidate(function () {
        return this.validateFields();
      }).setSubmit(function () {
        if (!this.validate()) return false;
        let data = [$f.serialize(), $f.data("all") ? "" : $form.find("input[name='key_m[]']:checked").serialize()].join("&");
        $__default.default.post(args.url, data, result => {
          var _result$error;
          if (Array.isArray(result)) {
            let successes = result.reduce((acc, cur) => acc + (cur.success ? 1 : 0), 0),
              failures = result.length - successes;
            if (successes > 0 && failures == 0) ew__default.default.alert(ew__default.default.language.phrase("PushNotificationSuccess").replace("%s", successes), "success");else if (successes == 0 && failures > 0) ew__default.default.alert(ew__default.default.language.phrase("PushNotificationFailure").replace("%f", failures));else if (successes == 0 && failures == 0) ew__default.default.alert(ew__default.default.language.phrase("NoSubscriptions").replace("%f", failures), "primary");else ew__default.default.alert(ew__default.default.language.phrase("PushNotificationSent").replace("%s", successes).replace("%f", failures), "primary");
          } else if (result != null && (_result$error = result.error) != null && _result$error.description) {
            ew__default.default.alert(result.error.description);
          }
        });
        return true;
      }).build();
      $f.data("form", frm);
    }
    ew__default.default.pushDialog = $dlg.modal("hide").find(".modal-title").html(args.hdr || target.dataset.caption).end().modal("show");
    return false;
  }

  /**
   * Check if push notification and service workers are supported by browser
   */
  function isPushNotificationSupported() {
    return "serviceWorker" in navigator && "PushManager" in window;
  }

  /**
   * Base 64 to Unit8Array
   */
  function urlBase64ToUint8Array(base64String) {
    const padding = "=".repeat((4 - base64String.length % 4) % 4),
      base64 = (base64String + padding).replace(/\-/g, "+").replace(/_/g, "/"),
      rawData = window.atob(base64),
      outputArray = new Uint8Array(rawData.length);
    for (let i = 0; i < rawData.length; ++i) outputArray[i] = rawData.charCodeAt(i);
    return outputArray;
  }

  /**
   * Send subscription to server
   */
  async function sendSubscriptionToServer(subscription, url) {
    const key = subscription.getKey("p256dh"),
      token = subscription.getKey("auth"),
      contentEncoding = (PushManager.supportedContentEncodings || ["aesgcm"])[0];
    let formData = new FormData();
    formData.set("endpoint", subscription.endpoint);
    formData.set("publicKey", key ? btoa(String.fromCharCode.apply(null, new Uint8Array(key))) : null);
    formData.set("authToken", token ? btoa(String.fromCharCode.apply(null, new Uint8Array(token))) : null);
    formData.set("contentEncoding", contentEncoding);
    let response = await ew__default.default.fetch(url, {
      method: "POST",
      body: formData
    });
    return response.json();
  }

  /**
   * Create subscription and send to server
   * @returns Promise that resolves to a PushSubscription object
   */
  async function createSubscription() {
    var _result$error2;
    let url = ew__default.default.getApiUrl([ew__default.default.API_PUSH_NOTIFICATION_ACTION, ew__default.default.API_PUSH_NOTIFICATION_SUBSCRIBE]),
      serviceWorkerReg = await navigator.serviceWorker.ready,
      subscription = await serviceWorkerReg.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: urlBase64ToUint8Array(ew__default.default.PUSH_SERVER_PUBLIC_KEY)
      }),
      result = await sendSubscriptionToServer(subscription, url);
    if (result != null && (_result$error2 = result.error) != null && _result$error2.description) ew__default.default.alert(result.error.description);
    return result.success ? subscription : null;
  }

  /**
   * Get subscription
   */
  async function getSubscription() {
    let serviceWorkerReg = await navigator.serviceWorker.ready;
    return serviceWorkerReg.pushManager.getSubscription();
  }

  /**
   * Set subscription
   * @param {PushSubscription} subscription
   */
  function setSubscription(subscription) {
    let $btn = $__default.default("#subscribe-notification").toggleClass("ew-enable-notification", !subscription).removeClass("disabled");
    $btn.find("i").toggleClass("fa-bell", !subscription).toggleClass("fa-bell-slash", !!subscription);
    let inst = bootstrap.Tooltip.getInstance($btn[0]);
    if (inst) inst._config.title = ew__default.default.language.phrase(subscription ? "DisableNotifications" : "EnableNotifications");
  }

  /**
   * Subscribe notifications
   *
   * @returns false
   */
  async function subscribeNotification() {
    if (Notification.permission !== "denied" && !ew__default.default.IS_SYS_ADMIN) {
      // Support non system administrator
      document.body.style.cursor = "wait";
      try {
        if (Notification.permission === "granted") {
          var _await$getSubscriptio, _document$getElementB;
          setSubscription((_await$getSubscriptio = await getSubscription()) != null ? _await$getSubscriptio : await createSubscription());
          if (!((_document$getElementB = document.getElementById("subscribe-notification")) != null && _document$getElementB.classList.contains("ew-enable-notification"))) ew__default.default.showToast(ew__default.default.language.phrase("NotificationsEnabled"), "success");
        } else if (Notification.permission === "default" && (await Notification.requestPermission()) == "granted") {
          var _document$getElementB2;
          setSubscription(await createSubscription());
          if (!((_document$getElementB2 = document.getElementById("subscribe-notification")) != null && _document$getElementB2.classList.contains("ew-enable-notification"))) ew__default.default.showToast(ew__default.default.language.phrase("NotificationsEnabled"), "success");
        }
      } catch (e) {
        ew__default.default.alert(e);
      } finally {
        document.body.style.cursor = "default";
      }
    }
    return false;
  }

  /**
   * Check subscription
   *
   * @returns true
   */
  async function checkSubscription() {
    $__default.default("#subscribe-notification").tooltip();
    if (Notification.permission == "granted") setSubscription(await getSubscription());else setSubscription(null);
    return true;
  }

  /**
   * Unsubscribe notifications
   *
   * @returns false
   */
  async function unsubscribeNotification() {
    ew__default.default.prompt({
      html: ew__default.default.language.phrase("DisableNotificationsConfirm")
    }, async res => {
      if (!res) return;
      document.body.style.cursor = "wait";
      try {
        var _result$error3;
        let subscription = await getSubscription();
        await subscription.unsubscribe();
        let url = ew__default.default.getApiUrl([ew__default.default.API_PUSH_NOTIFICATION_ACTION, ew__default.default.API_PUSH_NOTIFICATION_DELETE]),
          result = await sendSubscriptionToServer(subscription, url); // Delete subscription
        if (result != null && (_result$error3 = result.error) != null && _result$error3.description) {
          ew__default.default.alert(result.error.description);
          return false;
        }
        if (result.success) {
          // No subscription or deleted successfully
          setSubscription(null);
          ew__default.default.showToast(ew__default.default.language.phrase("NotificationsDisabled"), "success");
        }
      } catch (e) {
        ew__default.default.alert(e);
      } finally {
        document.body.style.cursor = "default";
      }
    });
    return false;
  }

  /**
   * Init web push notifications
   */
  loadjs.ready("foot", () => {
    if (ew__default.default.PUSH_SERVER_PUBLIC_KEY && isPushNotificationSupported()) {
      navigator.serviceWorker.register(ew__default.default.PATH_BASE + ew__default.default.SERVICE_WORKER, {
        scope: ew__default.default.PATH_BASE
      }).then(() => {
        checkSubscription();
        $__default.default("#subscribe-notification").on("click", function () {
          this.classList.contains("ew-enable-notification") ? subscribeNotification() : unsubscribeNotification();
        });
      }).catch(e => console.log(e));
    } else {
      $__default.default("#subscribe-notification").addClass("d-none");
    }
  });

  var webpush = {
    __proto__: null,
    pushNotificationDialogShow,
    isPushNotificationSupported,
    subscribeNotification,
    checkSubscription,
    unsubscribeNotification
  };

  /**
   * Map styles
   */

  /**
   * Show map
   * @param {Object} data Data of map
   */
  function show(data) {
    if (data.inited)
      // Already initiated
      return true;
    let latlng = data.latlng,
      $div = $__default.default("#" + data.id),
      useSingleMap = data.useSingleMap,
      showAllMarkers = data.showAllMarkers,
      useMarkerClusterer = data.useMarkerClusterer,
      ext = ew.maps[data.ext];
    if (useSingleMap) {
      // Use single map
      let id = data.id.replace(/^mp\d*_/, "mp_"); // Remove index from "m<n>_" prefix
      $div = $__default.default("#" + id);
      if (!$div[0]) {
        $div = $__default.default("<div></div>").attr("id", id).addClass("ew-single-map").height(data.singleMapHeight); // Create new $div for single map
        if (data.singleMapWidth) $div.width(data.singleMapWidth);
        $__default.default(".ew-grid, .ew-multi-column-grid").first()[data.showMapOnTop ? "before" : "after"]($div); // Insert before/after table
        data.map = ext.createMap($div[0], data);
        $div.data("map", data.map);
        $div.data("ext", data.ext);
        $div.data("bounds", showAllMarkers ? ext.createBounds() : null);
        $div.data("markerClusterer", useMarkerClusterer ? ext.createMarkerClusterer(data) : null);
      }
      data = Object.assign(data, $div.data()); // Merge data
    } else {
      if (!latlng) {
        // Location not found
        $div.addClass("d-none").html(data.status);
        return true;
      }
      $div.next(".ew-map-value").addClass("d-none"); // Hide view value
      if (!data.map) data.map = ext.createMap($div[0], data);
    }
    data.marker = ext.createMarker(data); // Create marker
    data.inited = true; // Initiated
    $__default.default(document).trigger("map", [data]);
    return true;
  }

  /**
   * All maps initiated
   */
  function done() {
    $__default.default(".ew-single-map").each(function () {
      let data = $__default.default(this).data();
      ew.maps[data.ext].fitBounds(data); // Fit bounds
    });

    $__default.default(document).trigger("maps");
  }

  /**
   * Init maps
   * @param {string} [ext] - Map type, e.g. 'googlemaps', 'leaflet'
   * @returns
   */
  function init(ext) {
    let promises = $__default.default(".ew-map").filter(function () {
      return !ext || $__default.default(this).data("ext") == ext;
    }).map(function (i) {
      let data = $__default.default(this).data();
      if (data.inited)
        // Already initiated
        return show(data);
      data.id = this.id; // Get ID
      data.address = (data.address || "").trim();
      if (data.address) {
        let geocodingDelay = data.geocodingDelay;
        return new Promise(resolve => {
          $__default.default.later(i * geocodingDelay, null, () => {
            // Set a timer for better performance
            resolve(ew.maps[data.ext].geocode(data).then(latlng => {
              data.latlng = latlng;
            }).catch(status => {
              data.status = status;
            }).finally(() => show(data)));
          });
        });
      } else {
        let latitude = data.latitude,
          longitude = data.longitude;
        if (latitude && !isNaN(latitude) && longitude && !isNaN(longitude)) data.latlng = ew.maps[data.ext].createLatLng(latitude, longitude);
        return show(data);
      }
    }).get();
    return Promise.all(promises).then(done);
  }

  /**
   * Init
   */
  $__default.default(function () {
    $__default.default("#ew-modal-dialog").on("load.ew", init);
    $__default.default(document).on("preview.ew", init);
  });

  var maps = {
    __proto__: null,
    show,
    done,
    init
  };

  ew.MOBILE_DETECT = new MobileDetect(window.navigator.userAgent);
  ew.IS_MOBILE = !!ew.MOBILE_DETECT.mobile();

  // Charts
  window.exportCharts = {}; // Per window

  // Extend
  Object.assign(ew, {
    MultiPage,
    Form,
    FormBuilder,
    Validators,
    maps
  }, functions, webpush, fullCalendar$1);
  var $document = $__default.default(document);

  // Init document
  loadjs.ready("load", function () {
    $__default.default.views.settings.debugMode(ew.DEBUG);
    ew.setSessionTimer();
    ew.initPage();
    $__default.default("#ew-modal-dialog").on("load.ew", ew.initPage);
    $__default.default("#ew-add-opt-dialog").on("load.ew", ew.initPage);
    var hash = ew.currentUrl.searchParams.get("hash");
    if (hash) $__default.default("html, body").animate({
      scrollTop: $__default.default("#" + hash).offset().top
    }, 800);
    $document.trigger("load.ew");
  });

  // Default "addoption" event (fired before adding new option to selection list)
  $document.on("addoption", function (e, args) {
    let row = args.data,
      // New row to be validated
      arp = args.parents; // Parent field values
    for (let i = 0, cnt = arp.length; i < cnt; i++) {
      // Iterate parent values
      let p = arp[i];
      if (!p.length)
        // Empty parent
        //continue; // Allow
        return args.valid = false; // Disallow
      let val = row["ff" + (i > 0 ? i + 1 : "")]; // Filter fields start from the 6th field
      if (!$__default.default.isUndefined(val) && !p.includes(String(val)))
        // Filter field value not in parent field values
        return args.valid = false; // Returns false if invalid
    }
  });

  // Click handler for buttons
  $document.on("click", "[data-ew-action]:not([data-ew-action=''])", function (e) {
    let data = Object.assign({}, $__default.default(this).data()),
      action = data.ewAction;
    if (!action) {
      return true;
    } else if (action == "none") {
      return false;
    } else if (action == "redirect") {
      return ew.redirect(data.url);
    } else if (action == "reload") {
      location.reload();
      return false;
    } else if (action == "submit") {
      delete data.ewAction;
      return ew.submitAction(e, data);
    } else if (action == "modal") {
      e.preventDefault();
      return data.ajax ? ew.inlineAction({
        evt: e,
        ...data
      }) : ew.modalDialogShow({
        evt: e,
        ...data
      });
    } else if (action == "inline" || action == "grid") {
      e.preventDefault();
      return ew.inlineAction({
        evt: e,
        ...data
      });
    } else if (action == "export") {
      return ew.export({
        evt: e,
        ...data
      });
    } else if (action == "layout") {
      return ew.toggleLayout(this);
    } else if (action == "language") {
      return ew.setLanguage(this);
    } else if (action == "filter") {
      return ew.filter(e);
    } else if (action == "sort") {
      return ew.sort(e, this.dataset);
    } else if (action == "refresh") {
      let url = ew.setLayout(this.dataset.url, false);
      if (url && url != "#") ew.refresh(fetch(url), this.dataset.context);
      return false;
    } else if (action == "email") {
      delete data.ewAction;
      return ew.emailDialogShow({
        evt: e,
        ...data
      });
    } else if (action == "set-action") {
      this.form.elements["action"].value = data.value;
    } else if (action == "drilldown") {
      return ew.showDrillDown(e, this, data.url, data.id, data.hdr);
    } else if (action == "add-option") {
      delete data.ewAction;
      return ew.addOptionDialogShow({
        evt: e,
        ...data
      });
    } else if (action == "search-type") {
      return ew.setSearchType(this);
    } else if (action == "search-operator") {
      ew.toggleSearchOperator(e, data.target, data.value);
    } else if (action == "search-toggle") {
      let panel = document.getElementById(this.dataset.form + "_search_panel");
      if (panel) {
        bootstrap.Collapse.getOrCreateInstance(panel).toggle();
        this.classList.toggle("active");
      }
    } else if (action == "highlight") {
      $__default.default("mark." + this.dataset.name).toggleClass("mark");
      this.classList.toggle("active");
    } else if (action == "inline-delete") {
      return ew.confirmDelete(this);
    } else if (action == "add-grid-row") {
      return ew.addGridRow(this);
    } else if (action == "delete-grid-row") {
      return ew.deleteGridRow(this, this.dataset.rowindex);
    } else if (action == "select-all") {
      ew.selectAll(this);
    } else if (action == "select-key") {
      ew.selectKey(e);
    } else if (action == "select-all-keys") {
      ew.selectAllKeys(this);
    } else if (action == "import") {
      return ew.importDialogShow({
        evt: e,
        hdr: data.hdr
      });
    } else if (action == "password") {
      return ew.togglePassword(e);
    } else if (action == "push") {
      return ew.pushNotificationDialogShow({
        evt: e,
        url: data.apiUrl
      });
    } else if (action == "enable-2fa") {
      return ew.enable2FA();
    } else if (action == "disable-2fa") {
      return ew.disable2FA();
    } else if (action == "send-otp") {
      var _document$getElementB;
      return ew.sendOTP((_document$getElementB = document.getElementById(data.account)) == null ? void 0 : _document$getElementB.value);
    } else if (action == "backup-codes") {
      return ew.showBackupCodes();
    } else if (action == "scroll-top") {
      $__default.default(document).scrollTop($__default.default('#top').offset().top);
      return false;
    }
  })
  // Change handler for selects
  .on("change", "[data-ew-action]:not([data-ew-action=''])", function (e) {
    let data = Object.assign({}, $__default.default(this).data());
    if (!data.ewAction) return true;
    let actions = data.ewAction.split(" ").filter(action => !!action);
    for (let action of actions) {
      if (action == "submit-form") {
        var _this$form;
        (_this$form = this.form) == null ? void 0 : _this$form.submit();
      } else if (action == "change-page-size") {
        let pageSize = ew.sameText(this.value, "ALL") ? "ALL" : ew.parseNumber(this.value);
        if (ew.convertToBool(data.ajax)) {
          let url = ew.setLayout(this.dataset.url, false) + "&" + this.name + "=" + pageSize;
          ew.refresh(fetch(url), this.dataset.context);
        } else {
          let url = ew.setSearchParam(this.dataset.url, this.name, pageSize);
          ew.redirect(url);
        }
        return false;
      } else if (action == "update-options") {
        ew.updateOptions.call(this);
      } else if (action == "autofill") {
        ew.autoFill(this);
      } else if (action == "search-operator") {
        return ew.searchOperatorChange(this);
      } else if (action == "language") {
        ew.setLanguage(this);
      } else if (action == "chart-order") {
        ew.updateChart(this);
      }
    }
  }).on("keydown", "[data-ew-action]:not([data-ew-action=''])", function (e) {
    let data = Object.assign({}, $__default.default(this).data()),
      action = data.ewAction;
    if (action == "change-page") {
      // ew-page-number
      if (e.key == "Enter") {
        if (ew.convertToBool(data.ajax)) {
          let url = ew.setLayout(this.dataset.url, false) + "&" + this.name + "=" + ew.parseNumber(this.value);
          ew.refresh(fetch(url), this.dataset.context);
        } else {
          ew.currentUrl.searchParams.set(this.name, ew.parseNumber(this.value));
          window.location = ew.sanitizeUrl(ew.currentUrl.toString());
        }
        return false;
      }
    }
  });

  // Click handler for row links
  $document.on("click", ".ew-row-link", e => e.stopPropagation());

  // Fix z-index of multiple modals
  $document.on("show.bs.modal", ".modal", function () {
    var zIndex = 1050 + $__default.default(".modal:visible").length;
    $__default.default(this).css("z-index", zIndex);
    setTimeout(function () {
      $__default.default(".modal-backdrop").not(".modal-stack").css("z-index", zIndex - 1).addClass("modal-stack");
    }, 0);
  });

  // Fix scrolling of multiple modals
  $document.on("hidden.bs.modal", ".modal", function () {
    $__default.default(".modal:visible").length && $__default.default("body").addClass("modal-open");
  });

  // Crop action for jQuery File Upload
  $__default.default.blueimp.fileupload.prototype.options.processQueue.unshift({
    action: "cropImage",
    // Action name
    fileTypes: "@loadImageFileTypes",
    // Reuse loadImageFileTypes
    maxFileSize: "@loadImageMaxFileSize",
    // Reuse loadImageMaxFileSize
    cropperOptions: "@",
    // Use global settings in ewcore.js, see https://github.com/fengyuanchen/cropperjs/blob/e969348d313dafe3416926125b21388cc67cefb1/README.md#options
    cropperCanvasOptions: "@",
    // Use global settings in ewcore.js, see https://github.com/fengyuanchen/cropperjs/blob/e969348d313dafe3416926125b21388cc67cefb1/README.md#getcroppedcanvasoptions
    disabled: "@disableImageCrop" // Use data-disable-image-crop attribute to enable/disable
  });

  $__default.default.widget("blueimp.fileupload", $__default.default.blueimp.fileupload, {
    processActions: {
      cropImage: function (data, options) {
        let file = data.files[data.index];
        if (options.disabled || $__default.default.isNumber(options.maxFileSize) && file.size > options.maxFileSize || options.fileTypes && !options.fileTypes.test(file.type)) return data;
        let url = URL.createObjectURL(file),
          dfd = $__default.default.Deferred(),
          that = this;
        let $dlg = ew.cropperDialog || $__default.default("#ew-cropper-dialog").on("shown.bs.modal", function () {
          $dlg.data("cropper", new Cropper(this.querySelector("#ew-crop-image"), options.cropperOptions));
        }).on("hidden.bs.modal", function () {
          var _$dlg$data;
          (_$dlg$data = $dlg.data("cropper")) == null ? void 0 : _$dlg$data.destroy();
          $dlg.removeData("cropper");
          $dlg.find("#ew-crop-image").attr("src", "data:image/png;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=");
        });
        ew.cropperDialog = $dlg;
        $dlg.find("#ew-crop-image").attr("src", url);
        $dlg.find(".ew-crop-btn").off("click").on("click", function () {
          if ($dlg.data("cropper")) {
            try {
              let canvas = $dlg.data("cropper").getCroppedCanvas(options.cropperCanvasOptions);
              canvas.toBlob(blob => {
                data.files[data.index] = new File([blob], file.name, {
                  type: file.type
                });
                dfd.resolveWith(that, [data]);
              }, file.type);
            } catch (error) {
              ew.alert(error);
              dfd.rejectWith(that, [data]);
            } finally {
              $dlg.modal("hide");
            }
          }
        });
        $dlg.find(".ew-skip-btn").off("click").on("click", function () {
          dfd.resolveWith(that, [data]);
          $dlg.modal("hide");
        });
        $dlg.modal("show");
        return dfd.promise();
      }
    }
  });

  // Extend jQuery
  $__default.default.extend({
    isBoolean: function (o) {
      return typeof o === 'boolean';
    },
    isNumber: function (o) {
      return typeof o === 'number' && isFinite(o);
    },
    isObject: function (o) {
      return o && (typeof o === 'object' || this.isFunction(o)) || false;
    },
    isString: function (o) {
      return typeof o === 'string';
    },
    isUndefined: function (o) {
      return typeof o === 'undefined';
    },
    isValue: function (o) {
      return this.isObject(o) || this.isString(o) || this.isNumber(o) || this.isBoolean(o);
    },
    isDate: function (o) {
      return this.type(o) === 'date' && o.toString() !== 'Invalid Date' && !isNaN(o);
    },
    later: function (when, o, fn, data, periodic) {
      when = when || 0;
      o = o || {};
      var m = fn,
        d = data,
        f,
        r;
      if (this.isString(fn)) m = o[fn];
      if (!m) return;
      if (!this.isUndefined(data) && !this.isArray(d)) d = [data];
      f = function () {
        m.apply(o, d || []);
      };
      r = periodic ? setInterval(f, when) : setTimeout(f, when);
      return {
        interval: periodic,
        cancel: function () {
          if (this.interval) {
            clearInterval(r);
          } else {
            clearTimeout(r);
          }
        }
      };
    }
  });

  /**
   * jQuery.fields() plugin
   *
   * @param {string|undefined} fldvar - Field variable name or undefined
   *  If field variable name, returns jQuery object of the specified field element(s).
   *  If unspecified, returns object of jQuery objects of all fields.
   * @returns jQuery object
   */
  $__default.default.fn.fields = function (fldvar) {
    // Note: fldvar has NO "x_" prefix
    let rec = {},
      id = this.attr("id"),
      obj = this[0],
      m = id.match(/^[xy](\d*)_/),
      f,
      tbl,
      infix;
    if (m) {
      // "this" is input element
      f = ew.getForm(obj); // form
      tbl = this.data("table"); // table var
      infix = m[1]; // row index
    } else if (obj != null && obj.htmlForm) {
      // "this" is form
      f = obj.$element; // form
      tbl = obj.id.replace(new RegExp("^f|" + obj.pageId + "$", "g"), ""); // table var
      infix = obj.htmlForm.dataset.rowindex; // row index
    }

    let selector = "[data-table" + (tbl ? "=" + tbl : "") + "][data-field" + (fldvar ? "=x_" + fldvar : "") + "]" + ($__default.default.isValue(infix) ? "[name^=x" + infix + "]" : "") + ":not(.ew-custom-option)";
    if (f && selector) {
      $__default.default(f).find(selector).each(function () {
        let key = this.dataset.field.substring(2),
          // "data-field"
          name = this.getAttribute("name");
        key = /^y_/.test(name) ? "y_" + key : key; // Use "y_fldvar" as key for 2nd search input
        rec[key] = rec[key] ? rec[key].add(this) : $__default.default(this); // Create jQuery object for each field
      });
    }

    return fldvar ? rec[fldvar] : rec;
  };
  $__default.default.fn.extend({
    // Get jQuery object of the row (<div> or <tr>)
    row: function () {
      var _this$data;
      let $row = this.closest("#r_" + ((_this$data = this.data("field")) == null ? void 0 : _this$data.substring(2)));
      if (!$row[0]) $row = this.closest(".ew-table > tbody > tr"); // Grid page
      return $row;
    },
    // Show/Hide field
    visible: function (v) {
      var _this$data2;
      let $p = this.closest("#r_" + ((_this$data2 = this.data("field")) == null ? void 0 : _this$data2.substring(2))); // Find the row
      if (!$p[0]) $p = this.closest("[id^=el]"); // Find the span
      if (typeof v != "undefined") {
        $p.toggle(v);
        return this;
      } else {
        return $el.is(":visible");
      }
    },
    // Get/Set field "readonly" attribute
    // Note: This attribute is ignored if the value of the type attribute is hidden, range, color, checkbox, radio, file, or a button type
    readonly: function (v) {
      if (typeof v != "undefined") {
        this.prop("readOnly", v);
        return this;
      } else {
        return this.prop("readOnly");
      }
    },
    // Get/Set field "disabled" attribute
    // Note: A disabled control's value isn't submitted with the form
    disabled: function (v) {
      if (typeof v != "undefined") {
        this.prop("disabled", v);
        return this;
      } else {
        return this.prop("disabled");
      }
    },
    // Get/Set field value(s)
    // Note: Return array if select-multiple
    value: function (v) {
      if (typeof v != "undefined") {
        ew.selectOption(this[0], v);
        return this;
      } else {
        return ew.getValue(this[0]);
      }
    },
    // Get field value as number
    toNumber: function () {
      return ew.parseNumber(this.value());
    },
    // Get field value as Luxon object
    toDate: function () {
      var _ew$vars$tables, _ew$vars$tables$table, _ew$vars$tables$table2, _ew$vars$tables$table3;
      let data = this.data(),
        table = data.table,
        field = data.field.replace(/^[xy]_/, ""),
        format = (_ew$vars$tables = ew.vars.tables) == null ? void 0 : (_ew$vars$tables$table = _ew$vars$tables[table]) == null ? void 0 : (_ew$vars$tables$table2 = _ew$vars$tables$table.fields) == null ? void 0 : (_ew$vars$tables$table3 = _ew$vars$tables$table2[field]) == null ? void 0 : _ew$vars$tables$table3.clientFormatPattern;
      return ew.parseDateTime(this.value(), format);
    },
    // Get field value as native Date object
    toJsDate: function () {
      return this.toDate().toJSDate();
    }
  });

  // Improve Bootstrap Dropdown
  bootstrap.Dropdown.prototype.toggle = function () {
    if (!this._element.classList.contains("ew-dropdown-toggle")) {
      if (this._element.closest("[data-widget]")) $__default.default(this._element).on("click.ew.dropdown", e => {
        bootstrap.Dropdown.clearMenus(e.originalEvent); // Clear menus first because this click event will not propagate to the document
        e.stopPropagation(); // Stop propagate or the event will toggle row of expandable-table
      });
      // Move the menu to document body if menu inside responsive table
      if (this._menu.closest(".table-responsive")) {
        let container = this._menu.closest(".popover-body, .modal-body, .offcanvas-body, body");
        container.appendChild(this._menu);
        let menu = this._menu;
        if (!this._element.id) this._element.id = "dropdownbtn" + ew.random();
        menu.setAttribute('aria-labelledby', this._element.id);
        function callback(mutationList) {
          mutationList.forEach(mutation => {
            switch (mutation.type) {
              case "attributes":
                if (mutation.target.classList.contains("show")) menu.classList.add("d-block");else menu.classList.remove("d-block");
                break;
            }
          });
        }
        let observer = new MutationObserver(callback);
        observer.observe(this._parent, {
          attributeFilter: ["class"],
          attributeOldValue: false,
          subtree: false
        });
      }
      this._element.classList.add("ew-dropdown-toggle");
    }
    return this._isShown() ? this.hide() : this.show();
  };

  var isNil = (value) => {
      return value === null || value === undefined;
  };

  var isNil$1 = isNil;

  var isFunction = (value) => {
      return typeof value === 'function';
  };

  var isFunction$1 = isFunction;

  var isNaNNumber = (value) => {
      return typeof value === 'number' && isNaN(value);
  };

  var isNaNNumber$1 = isNaNNumber;

  var isFiniteNumber = (value) => {
      return typeof value == 'number' && isFinite(value);
  };

  var isFiniteNumber$1 = isFiniteNumber;

  /**
   * Optimized for performance
   */
  const multiplyByPowerOfTen = (number, powerOfTenExponent) => {
      if (!isFiniteNumber$1(number))
          return NaN;
      const numAsString = '' + number;
      const indexOfE = numAsString.indexOf('e');
      if (indexOfE === -1) {
          return +(numAsString + 'e' + powerOfTenExponent);
      }
      else {
          return +(numAsString.slice(0, indexOfE) + 'e' + (+numAsString.slice(indexOfE + 1) + powerOfTenExponent));
      }
  };

  var multiplyByPowerOfTen$1 = multiplyByPowerOfTen;

  /**
   * Only handles direct power of ten (only integer exponents)
   */
  const log10 = (numberThatIsPowerOfTen) => {
      return Math.round(Math.log(numberThatIsPowerOfTen) * Math.LOG10E);
  };

  var log10$1 = log10;

  const toObject = (arr, entriesResolver) => {
      const object = {};
      for (let i = 0; i < arr.length; ++i) {
          if (i in arr) {
              const [key, value] = entriesResolver(arr[i], i);
              object[key] = value;
          }
      }
      return object;
  };

  var toObject$1 = toObject;

  // <i> Extracted from https://stackoverflow.com/questions/3446170/escape-string-for-use-in-javascript-regex
  const escapeRegexString = (string) => {
      return string.replace(/[.*+?^${}()|[\]\\]/g, match => `\\${match}`);
  };

  var escapeRegexString$1 = escapeRegexString;

  /**
   * This function doesn't work with non-primitive arguments
   */
  const memoize = (fn) => {
      const cache = {};
      return function (...args) {
          const cacheKey = args.length > 1 ? args.join('-(:-:)-') : args[0];
          if (cacheKey in cache) {
              return cache[cacheKey];
          }
          const result = fn.apply(this, args);
          cache[cacheKey] = result;
          return result;
      };
  };

  var memoize$1 = memoize;

  const powerOf10LookupObject = (() => {
      const object = {};
      // 1 <= x <= Infinity (positive exponent)
      let additionalZeros = '';
      let currentValue;
      while (currentValue !== Infinity) {
          currentValue = +('1' + additionalZeros);
          object[currentValue] = true;
          additionalZeros += '0';
      }
      // 0 <= x < 1 (negative exponent)
      additionalZeros = '';
      currentValue = undefined;
      while (currentValue !== 0) {
          currentValue = +('0.' + additionalZeros + '1');
          object[currentValue] = true;
          additionalZeros += '0';
      }
      return object;
  })();
  const isPowerOfTen = (number) => {
      return !!powerOf10LookupObject[number];
  };

  var isPowerOfTen$1 = isPowerOfTen;

  const toBase = (value, valueUnit, unitScale) => {
      if (!isFiniteNumber$1(value) || valueUnit === unitScale.base)
          return value;
      if (!(valueUnit in unitScale.scale))
          return NaN;
      const toBaseMultiplier = unitScale.scale[valueUnit] || 1;
      return isPowerOfTen$1(toBaseMultiplier)
          ? multiplyByPowerOfTen$1(value, log10$1(toBaseMultiplier))
          : value * toBaseMultiplier;
  };
  const convertUnit = (value, originUnit, targetUnit, unitScale) => {
      if (!isFiniteNumber$1(value) || originUnit === targetUnit)
          return value;
      const valueAsBase = toBase(value, originUnit, unitScale);
      const resolvedScale = Object.assign(Object.assign({}, unitScale.scale), { [unitScale.base]: 1 });
      if (isNaN(valueAsBase) || !(originUnit in resolvedScale) || !(targetUnit in resolvedScale))
          return NaN;
      const conversionFactorFromBase = unitScale.scale[targetUnit] || 1;
      return isPowerOfTen$1(conversionFactorFromBase)
          ? multiplyByPowerOfTen$1(valueAsBase, -log10$1(conversionFactorFromBase))
          : valueAsBase / conversionFactorFromBase;
  };
  /**
   * Looks through every possibility for the 'best' available unit.
   * i.e. Where the value has the fewest numbers before the decimal point,
   * but is still higher than 1.
   */
  const toBest = (value, originUnit, unitScale, options) => {
      const resolvedOptions = Object.assign({ exclude: [], cutOffNumber: 1 }, options);
      let best = null;
      const scale = unitScale.scale;
      Object.keys(scale).sort((a, b) => scale[a] - scale[b]).forEach((scaleUnit) => {
          const isIncluded = resolvedOptions.exclude.indexOf(scaleUnit) === -1;
          if (!isIncluded)
              return;
          const result = convertUnit(value, originUnit, scaleUnit, unitScale);
          const absoluteResult = Math.abs(result);
          if (!best || (absoluteResult >= resolvedOptions.cutOffNumber && absoluteResult < Math.abs(best[0]))) {
              best = [result, scaleUnit];
          }
      });
      return best || [value, originUnit];
  };
  const unitScale = (unitScaleDefinition) => {
      return {
          toBase: (value, unit) => {
              return toBase(value, unit, unitScaleDefinition);
          },
          convert: (value, originUnit, targetUnit) => {
              return convertUnit(value, originUnit, targetUnit, unitScaleDefinition);
          },
          toBest: (value, originUnit, options) => {
              return toBest(value, originUnit, unitScaleDefinition, options);
          },
          scaleDefinition: unitScaleDefinition,
      };
  };

  /**
   * Short version of mozilla polyfill:
   * <i> See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/repeat
   */
  const stringRepeat = (str, count) => {
      // return (str.repeat && str.repeat(count)) || new Array(count + 1).join(str);
      if (count < 1)
          return '';
      let result = '';
      let pattern = str;
      while (count > 1) {
          if (count & 1)
              result += pattern;
          count >>>= 1, pattern += pattern;
      }
      return result + pattern;
  };

  var stringRepeat$1 = stringRepeat;

  const baseCreateUnitScaleFromLocaleAbbreviations = (str) => {
      if (!str) {
          return unitScale({ base: '', scale: {} });
      }
      const scale = str.split('|');
      const scaleDefinition = { ['']: 1 };
      scale.forEach((scaleItem, scaleItemIndex) => {
          if (!scaleItem)
              return;
          scaleDefinition[scaleItem] = +(1 + stringRepeat$1('0', scaleItemIndex));
      });
      return unitScale({ base: '', scale: scaleDefinition });
  };
  const createUnitScaleFromLocaleAbbreviations = memoize$1(baseCreateUnitScaleFromLocaleAbbreviations);

  var createUnitScaleFromLocaleAbbreviations$1 = createUnitScaleFromLocaleAbbreviations;

  const replaceNumeralSystemWithLatinNumbers = (numericStringWithExtraInfo, numeralSystemMap) => {
      if (!numeralSystemMap || numeralSystemMap.length !== 10)
          return numericStringWithExtraInfo;
      const numericStringLength = numericStringWithExtraInfo.length;
      const numeralSystemToLatinSystemMap = toObject$1(numeralSystemMap, (digit, digitIndex) => [digit.replace(/\u200e/g, ''), '' + digitIndex]);
      let output = '';
      for (let numericStringIndex = 0; numericStringIndex < numericStringLength; numericStringIndex++) {
          const char = numericStringWithExtraInfo[numericStringIndex];
          output += numeralSystemToLatinSystemMap[char] || char;
      }
      return output;
  };
  const getScalingFactorFromAbbreviations = (stringOriginal, options) => {
      var _a;
      const scale = createUnitScaleFromLocaleAbbreviations$1((_a = options.locale) === null || _a === void 0 ? void 0 : _a.abbreviations);
      const abbreviationsSortedByLengthDesc = Object.keys(scale.scaleDefinition.scale).sort((a, b) => b.length - a.length);
      let abbreviationScalingFactor = 1;
      for (const abbreviation of abbreviationsSortedByLengthDesc) {
          const scapedAbbreviationForRegex = escapeRegexString$1(abbreviation);
          const regexp = new RegExp('[^a-zA-Z]'
              + `(${scapedAbbreviationForRegex})|(${scapedAbbreviationForRegex.replace(/\u200e/g, '')})`
              + '(?:\\)(?:\\))?)?$');
          if (stringOriginal.match(regexp)) {
              abbreviationScalingFactor = scale.toBase(abbreviationScalingFactor, abbreviation);
              break;
          }
      }
      return abbreviationScalingFactor;
  };
  // unformats numbers separators, decimals places, signs, abbreviations
  const formattedStringToNumber = (inputString, options) => {
      var _a;
      const locale = options.locale;
      const stringOriginal = inputString;
      let value;
      // Replace special digits with latin digits
      const stringWithLatinDigits = replaceNumeralSystemWithLatinNumbers(inputString, (_a = options.locale) === null || _a === void 0 ? void 0 : _a.numeralSystem);
      if (options.zeroFormat && stringWithLatinDigits === options.zeroFormat) {
          value = 0;
      }
      else if (options.nullFormat && stringWithLatinDigits === options.nullFormat || !stringWithLatinDigits.replace(/[^0-9]+/g, '').length) {
          value = null;
      }
      else {
          // Replaces the locale decimal delimiter with a dot (.)
          const decimalDelimiterFromLocale = locale.delimiters.decimal;
          const stringWithDotDecimalDelimiter = decimalDelimiterFromLocale === '.'
              ? stringWithLatinDigits
              : stringWithLatinDigits.replace(/\./g, '').replace(decimalDelimiterFromLocale, '.');
          // Determines the scaling factor from the abbreviations (if has abbreviations)
          const abbreviationScalingFactor = getScalingFactorFromAbbreviations(stringOriginal, options);
          // Check for negative number
          const negativeFactor = (stringWithDotDecimalDelimiter.split('-').length
              + Math.min(stringWithDotDecimalDelimiter.split('(').length - 1, stringWithDotDecimalDelimiter.split(')').length - 1)) % 2 ? 1 : -1;
          // Remove non numbers
          const numberAsString = stringWithDotDecimalDelimiter.replace(/[^0-9.]+/g, '');
          value = negativeFactor * multiplyByPowerOfTen$1(+numberAsString, log10$1(abbreviationScalingFactor));
      }
      return value;
  };

  var formattedStringToNumber$1 = formattedStringToNumber;

  const locale = {
      code: 'en',
      delimiters: {
          thousands: ',',
          decimal: '.',
      },
      abbreviations: '|||K|||M|||B|||T',
      ordinal: number => {
          const b = number % 10;
          return (Math.floor(number % 100 / 10) === 1)
              ? 'th'
              : b === 1
                  ? 'st'
                  : b === 2
                      ? 'nd'
                      : b === 3
                          ? 'rd'
                          : 'th';
      },
  };

  var locale$1 = locale;

  function merge(...args) {
      const newObject = {};
      const argsLength = args.length;
      for (let i = 0; i < argsLength; i++) {
          for (const key in args[i])
              newObject[key] = args[i][key];
      }
      return newObject;
  }

  var isObject = (value) => {
      return typeof value === 'object' && value !== null;
  };

  var isObject$1 = isObject;

  var isString = (value) => {
      return typeof value === 'string';
  };

  var isString$1 = isString;

  const truncateNumber = (value) => {
      return value < 0 ? Math.ceil(value) : Math.floor(value);
  };

  var truncateNumber$1 = truncateNumber;

  const getPatternParts = (patternMask) => {
      let isInEscapedPart = false;
      let currentEscapedWord = '';
      const parts = [];
      for (let i = 0; i < patternMask.length; i++) {
          const char = patternMask.charAt(i);
          if (char === "'" && !isInEscapedPart) {
              isInEscapedPart = true;
              currentEscapedWord = '';
          }
          else if (char === "'" && isInEscapedPart && patternMask.charAt(i - 1) !== "\\") {
              isInEscapedPart = false;
              parts.push({ escaped: true, value: currentEscapedWord });
          }
          else if (isInEscapedPart) {
              currentEscapedWord += char;
          }
          else {
              if (parts.length && !parts[parts.length - 1].escaped) {
                  parts[parts.length - 1].value += char;
              }
              else {
                  parts.push({ escaped: false, value: char });
              }
          }
      }
      return parts;
  };
  /**
   * Checks only the pattern parts that are not escaped
   */
  const patternIncludes = (patternMask, search) => {
      return patternRemoveEscapedText(patternMask).indexOf(search) !== -1;
  };
  /**
   * Replaces only the pattern parts that are not escaped
   */
  const patternReplace = (patternMask, searchValue, replaceValue) => {
      return getPatternParts(patternMask)
          .map(e => e.escaped ? `'${e.value}'` : e.value.replace(searchValue, _ => replaceValue))
          .join('');
  };
  const patternRemoveEscapedText = (patternMask) => {
      return getPatternParts(patternMask)
          .filter(e => !e.escaped)
          .map(e => e.value)
          .join('');
  };
  const patternStripAndNormalizeEscapedText = (patternMask) => {
      return getPatternParts(patternMask)
          .map(e => e.escaped ? e.value.replace(/\\'/g, "'") : e.value)
          .join('');
  };

  const stringIncludes = (str, search) => {
      return str.indexOf(search) !== -1;
  };

  var stringIncludes$1 = stringIncludes;

  /**
   * What does it look for in the pattern?
   *     '(' | '+' | '-'
   * What does it remove from the pattern?
   *     '(' | ')' | '+' | '-'
   * What options does it provide?
   *     - negativeParentheses (if negative value should be wrapped between parentheses)
   *     - forceSign (is positive values should have a + sign)
   * How will it transform the output?
   *     - Parentheses:
   *         '-23.58' & '(0.00)'  =>  '(23.58)'
   *         '-23.58' & '( 0.00 )'  =>  '( 23.58 )'
   *         '-23.58' & '(  0.00 ) '  =>  ' (  23.58 )'
   *     - Sign:
   *         '12.34' & '+0.0'  =>  '+12.34'
   *         '-12.34' & '+0.0'  =>  '-12.34'
   *
   * <i> If '+' is somewhere in the pattern, it will set the '+' sign for positive numbers
   *     and same for negative numbers.
   * <i> If '-' is somewhere in the pattern, it will place the negative sign in the defined position.
   *     But it won't still set the sign for positive numbers.
   * <i> Checks if we should use parentheses for negative number or if we should prefix with a sign.
   *     If both are present we default to parentheses.
   */
  const signRule = (pattern) => {
      const patternWithoutEscapedText = patternRemoveEscapedText(pattern);
      const negativeParentheses = stringIncludes$1(patternWithoutEscapedText, '(') && stringIncludes$1(patternWithoutEscapedText, ')');
      const forceSign = !negativeParentheses && stringIncludes$1(patternWithoutEscapedText, '+');
      let outputPatternMask = pattern;
      outputPatternMask = patternReplace(outputPatternMask, '(', `'ɵnps'`);
      outputPatternMask = patternReplace(outputPatternMask, ')', `'ɵnpe'`);
      outputPatternMask = patternReplace(outputPatternMask, /(-|\+)/, `'ɵs'`);
      return [outputPatternMask, { negativeParentheses, forceSign }];
  };

  var signRule$1 = signRule;

  /**
   * Checks if abbreviation is wanted
   * <i> Applied only when 'a' is present.
   * <i> If 'a' is followed by 'k' | 'm' | 'b' | 't', then, it will force the abbreviation to be the specified
   *     unit. (e.g. (123456.78, '0,0.00am')  =>  '0.12M')
   */
  const abbreviationRule = (patternMask) => {
      let compactUnit = null; // force abbreviation
      let compact = false;
      // If it includes 'a' means it should be abbreviated (only if at least includes 'a')
      if (patternIncludes(patternMask, 'a')) {
          compact = true;
          const patternWithoutEscapedText = patternRemoveEscapedText(patternMask);
          const abbreviationRegExpResult = patternWithoutEscapedText.match(/a(k|m|b|t)?/);
          compactUnit = !!abbreviationRegExpResult ? abbreviationRegExpResult[1] : null;
      }
      let outputPatternMask = patternMask;
      outputPatternMask = patternReplace(outputPatternMask, /a(k|m|b|t)?/, `'ɵa'`);
      return [outputPatternMask, { compact, compactUnit, compactAuto: compact && !compactUnit }];
  };

  var abbreviationRule$1 = abbreviationRule;

  /**
   * Faster version of String.prototype.split that only handles splitting in two parts
   */
  const splitStringInTwoParts = (str, separator) => {
      if (!str)
          return ['', ''];
      const indexOfSearchChar = str.indexOf(separator);
      if (indexOfSearchChar === -1) {
          return [str, ''];
      }
      else {
          return [str.slice(0, indexOfSearchChar), str.slice(indexOfSearchChar + 1)];
      }
  };

  var splitStringInTwoParts$1 = splitStringInTwoParts;

  const countChars = (string, char) => {
      return !string ? 0 : string.split('').filter(stringChar => stringChar === char).length;
  };
  /**
   * Fraction digits (decimals) count rule (minimum and maximum fraction digits)
   * <i> Optional fraction digits would go always after the forced ones
   */
  const decimalPlacesRule = (patternMask) => {
      const patternWithoutEscapedText = patternRemoveEscapedText(patternMask);
      const patternPrecisionPart = splitStringInTwoParts$1(patternWithoutEscapedText, '.')[1];
      let minimumFractionDigits = 0;
      let maximumFractionDigits = 0;
      if (!!patternPrecisionPart) {
          const trimmedPatternPrecisionPart = patternPrecisionPart.trim();
          if (stringIncludes$1(trimmedPatternPrecisionPart, '[')) {
              // If it contains optional fraction digits
              const patternPrecisionPartWithoutClosingBracket = trimmedPatternPrecisionPart.replace(']', '');
              // Isolates forced (left) vs optional (right) decimals
              const precisionSplitted = splitStringInTwoParts$1(patternPrecisionPartWithoutClosingBracket, '[');
              minimumFractionDigits = countChars(precisionSplitted[0], '0');
              maximumFractionDigits = minimumFractionDigits + countChars(precisionSplitted[1], '0');
          }
          else if (stringIncludes$1(trimmedPatternPrecisionPart, '#')) {
              // If it contains optional fraction digits marked with '#'
              minimumFractionDigits = countChars(trimmedPatternPrecisionPart.split('#')[0], '0');
              maximumFractionDigits = trimmedPatternPrecisionPart.length;
          }
          else if (stringIncludes$1(trimmedPatternPrecisionPart, 'X')) {
              // If it contains no-maximum fraction digits marked with 'X'
              minimumFractionDigits = countChars(trimmedPatternPrecisionPart.split('X')[0], '0');
              maximumFractionDigits = 500;
          }
          else {
              const fractionDigits = countChars(trimmedPatternPrecisionPart.split(' ')[0], '0');
              minimumFractionDigits = fractionDigits;
              maximumFractionDigits = fractionDigits;
          }
      }
      return { minimumFractionDigits, maximumFractionDigits };
  };

  var decimalPlacesRule$1 = decimalPlacesRule;

  /**
   * Check for optional decimals.
   *
   * <i> 'optionalDecimals' This would mean that:
   *     - In case the number (value) HAS decimals (e.g. 55.34), then it would display the fixed amount of defined
   *       decimals (e.g. '0[.]000' => 3 fixed decimals), but, if the number is an straight integer, then it won't
   *       display any decimals.
   * <i> It could also accept optional decimals afterwards. So for the case '0[.]00##':
   *     - If it is an integer, displays only an integer:  23 => '23'
   *     - If it has 1 decimal, displays 2 decimals:       23.4 => '23.40'
   *     - If it has 3 decimals, displays 3 decimals:      23.456 => '23.456'
   */
  const optionalDecimalPlacesRule = (patternMask) => {
      let optionalFractionDigits = false;
      let outputPatternMask = patternMask;
      if (patternIncludes(patternMask, '[.]')) {
          optionalFractionDigits = true;
          outputPatternMask = patternReplace(outputPatternMask, '[.]', '.');
      }
      return [outputPatternMask, { optionalFractionDigits }];
  };

  var optionalDecimalPlacesRule$1 = optionalDecimalPlacesRule;

  /**
   * <i> The regExp tests for:
   *     - 0.00##X
   *     - 0,0.00##X
   *     - #.00##X (without leading zeros)
   *     - #,#.00##X (without leading zeros)
   */
  const numberPositionRule = (patternMask) => {
      const numberPartRegExp = /((((0|#)+,)?(0|#)+(\.([0#X]|\[0+\])+)?){1})/;
      return patternReplace(patternMask, numberPartRegExp, `'ɵn'`);
  };
  /**
   * Minimum leading integer digits rule
   * This will define the minimum amount of digits on the integer part (left-most grouped zeroes).
   *     - (12.34, '0000.0') =>  '0012.3'
   *     - (12.34, '0000,0.0')  =>  '0,012.3'
   *     - (0.34, '#.0') => '.3'
   *     - (1.34, '#.0') => '1.3'
   *
   * <i> It always pick the left-most amount of zeros, so:
   *     - If pattern has NO thousands separator ('000.0'), then the amount at the left of the DOT is used.
   *     - If pattern HAS thousands separator ('00,0.0'), then the amount at the left of the COMMA is used.
   * <i> This will remove the integer zero for numbers between 1 and -1 (e.g. 0.23 or -0.5)
   *     - If pattern integer part is option (0.24, '#.00') => '.24'
   */
  const minimumIntegerDigitsRule = (patternMask) => {
      const patternMaskWithoutEscapedText = patternRemoveEscapedText(patternMask);
      const patternMaskIntegerPart = patternMaskWithoutEscapedText.split('.')[0].split(',')[0];
      // If it has '#' in the integer part, sets the minimumIntegerDigits to 0
      if (/#/g.test(patternMaskIntegerPart)) {
          return 0;
      }
      return (patternMaskIntegerPart.match(/0/g) || []).length;
  };
  // If sign is not included, put sign at the left of the number
  const addSignPositionIfItDoesNotExists = (patternMask) => {
      if (stringIncludes$1(patternMask, `'ɵs'`) || stringIncludes$1(patternMask, `'ɵnps'`))
          return patternMask;
      return patternMask.replace(`'ɵn'`, _ => `'ɵs''ɵn'`);
  };
  const baseParsePattern = (inputPattern) => {
      const resolvedInputPattern = isString$1(inputPattern) && inputPattern || '0,0.##########';
      const [patternMaskAfterSignRule, signRules] = signRule$1(resolvedInputPattern);
      const [patternMaskAfterAbbreviationRule, abbreviationRules] = abbreviationRule$1(patternMaskAfterSignRule);
      const [patternMaskAfterOptionalDecimalPlacesRule, optionalDecimalPlacesRules] = optionalDecimalPlacesRule$1(patternMaskAfterAbbreviationRule);
      const outputPatternMask = patternMaskAfterOptionalDecimalPlacesRule;
      const outputPatternMaskWithoutEscapedText = patternRemoveEscapedText(outputPatternMask);
      const decimalPlacesRules = decimalPlacesRule$1(outputPatternMask);
      const minimumIntegerDigits = minimumIntegerDigitsRule(outputPatternMask);
      const grouping = outputPatternMaskWithoutEscapedText.indexOf(',') > -1;
      const patternMaskAfterHandlingNumberPosition = numberPositionRule(outputPatternMask);
      const patternMaskWithEnsuredSignPosition = addSignPositionIfItDoesNotExists(patternMaskAfterHandlingNumberPosition);
      const patternMask = patternMaskWithEnsuredSignPosition;
      return Object.assign(Object.assign(Object.assign(Object.assign(Object.assign({}, signRules), abbreviationRules), optionalDecimalPlacesRules), decimalPlacesRules), { grouping,
          minimumIntegerDigits,
          patternMask });
  };
  const parsePattern = memoize$1(baseParsePattern);

  var parsePattern$1 = parsePattern;

  /**
   * <i> Checks if value is negative, and removes the sign in case it exists
   * Expects: a value as string, with or without the minus sign, AND NOTHING ELSE.
   * Returns: same as the input but without the minus sign in case it had.
   */
  const removeSignIfExists = (valueAsString) => {
      return valueAsString[0] === '-' ? valueAsString.slice(1) : valueAsString;
  };

  var removeSignIfExists$1 = removeSignIfExists;

  /**
   * This process only is applied for automatic abbreviation ('a'), and only in the rounding
   * bubbling cases where the scale has to be recomputed.
   * E.g.
   *     formatNumber(999960, 0.0a') // Would return '1000.0k' instead of '1.0m' without this rescaling-fix
   *
   * It only applies rescaling if the absolute value of 'the already scaled and rounded value' (1.87, 'k') is greater or
   * equal to 1000, and the abbreviation is not greater or equal to trillion.
   *
   * <i> After initial scaling, value shouldn't be greater than 1000, unless it is a trillion.
   * <i> The resulting decimal part, will be always 0, as this will be executed only on the corner cases,
   *     that results from rounding bubbling. This is why decimal part is ignored.
   */
  const rescaleRoundedValue = (value, currentAbbreviationScale, patternRules, options) => {
      const { compact, compactAuto } = patternRules;
      const { abbreviations } = options.locale;
      if (!compact || !compactAuto) {
          return [value, currentAbbreviationScale];
      }
      const scale = createUnitScaleFromLocaleAbbreviations$1(abbreviations);
      const [newScaledValue, newScaledValueUnit] = scale.toBest(value, currentAbbreviationScale || '');
      return [newScaledValue, newScaledValueUnit];
  };

  var rescaleRoundedValue$1 = rescaleRoundedValue;

  /**
   * If abbreviation is forced, looks for the closest (in terms of power of ten) abbreviation in the current locale
   *     k === 10 ** 3
   *     m === 10 ** 6
   *     b === 10 ** 9
   *     t === 10 ** 12
   */
  const resolveForcedAbbreviationUnit = (forcedAbbreviationUnit, abbreviationsFromLocale, value) => {
      // Record<AbbreviationSymbol, PowerOfTenExponent>
      const forcedScaleMap = { k: 3, m: 6, b: 9, t: 12 };
      const targetPowerOfTenExponent = forcedScaleMap[forcedAbbreviationUnit];
      const scaleDefinitionFromLocale = (abbreviationsFromLocale === null || abbreviationsFromLocale === void 0 ? void 0 : abbreviationsFromLocale.split('|')) || [];
      let closestPowerOfTenWithAvailableAbbreviation = null;
      for (let distanceFromTarget = 0; distanceFromTarget < scaleDefinitionFromLocale.length; distanceFromTarget++) {
          if (!scaleDefinitionFromLocale[targetPowerOfTenExponent - distanceFromTarget])
              continue;
          closestPowerOfTenWithAvailableAbbreviation = targetPowerOfTenExponent - distanceFromTarget;
          break;
      }
      if (closestPowerOfTenWithAvailableAbbreviation === null) {
          return [value, null];
      }
      return [
          multiplyByPowerOfTen$1(value, -closestPowerOfTenWithAvailableAbbreviation),
          scaleDefinitionFromLocale[closestPowerOfTenWithAvailableAbbreviation],
      ];
  };
  const scaleValueWithAbbreviation = (value, patternRules, options) => {
      const { compact, compactUnit } = patternRules;
      const { abbreviations } = options.locale;
      if (!compact)
          return [value, null];
      if (!!compactUnit) {
          return resolveForcedAbbreviationUnit(compactUnit, abbreviations, value);
      }
      /**
       * If abbreviation is automatic, resolves the abbreviation to the best (where the value has
       * the fewest numbers before the decimal point, but is still higher than 1).
       */
      const scale = createUnitScaleFromLocaleAbbreviations$1(abbreviations);
      const [scaledValue, localizedUnit] = scale.toBest(value, '');
      return [scaledValue, localizedUnit || null];
  };

  var scaleValueWithAbbreviation$1 = scaleValueWithAbbreviation;

  const roundNumber = (number, precision, roundingFunction) => {
      const resolvedPrecision = precision || 0;
      const resolvedRoundingFunction = roundingFunction || Math.round;
      const scaledValueForRounding = multiplyByPowerOfTen$1(number, resolvedPrecision);
      const roundedScaledValue = resolvedRoundingFunction(scaledValueForRounding);
      const roundedValue = multiplyByPowerOfTen$1(roundedScaledValue, -resolvedPrecision);
      return roundedValue;
  };

  var roundNumber$1 = roundNumber;

  /**
   * The result from toFixed can contain an exponent for big numbers (e.g. 1.12345671234567e+50).
   * <!> Only handles positive exponents.
   */
  const formatPositiveExponentResult = (valueAsString) => {
      const [significand, exponent] = splitStringInTwoParts$1(valueAsString, 'e');
      const exponentAsNumber = +exponent;
      if (exponentAsNumber < 0)
          return valueAsString;
      const [integerPartOfSignificand, fractionalPartOfSignificand] = splitStringInTwoParts$1(significand, '.');
      const numberOfZerosToAdd = exponentAsNumber - fractionalPartOfSignificand.length;
      return `${integerPartOfSignificand}${fractionalPartOfSignificand}${stringRepeat$1('0', numberOfZerosToAdd)}`;
  };
  /**
   * The result from toFixed can contain an exponent for small numbers (e.g. 1.123e-87).
   * <i> Only handles negative exponents
   */
  const formatNegativeExponentResult = (value, exponentAsNumber, significandAsString) => {
      const negativeExponentAbsoluteValue = Math.abs(exponentAsNumber);
      const [integerPartOfSignificand, fractionalPartOfSignificand] = splitStringInTwoParts$1(significandAsString, '.');
      const absoluteIntegerPartOfSignificand = integerPartOfSignificand[0] === '-' ? integerPartOfSignificand.slice(1) : integerPartOfSignificand;
      let outputIntegerPartOfSignificand = absoluteIntegerPartOfSignificand;
      let outputFractionalPartOfSignificand = fractionalPartOfSignificand;
      for (let i = 0; i < negativeExponentAbsoluteValue; i += 1) {
          // Consider using array.shift
          const firstCharInIntegerPart = outputIntegerPartOfSignificand[0] || '';
          outputIntegerPartOfSignificand = outputIntegerPartOfSignificand.slice(0, outputIntegerPartOfSignificand.length - 1);
          outputFractionalPartOfSignificand = (firstCharInIntegerPart || '0') + outputFractionalPartOfSignificand;
      }
      return `${value < 0 ? '-' : ''}${outputIntegerPartOfSignificand || 0}.${outputFractionalPartOfSignificand}`;
  };
  /**
   * Like Number.prototype.toString() but excluding the exponential info for small and big numbers.
   * e.g.
   *     Small numbers:
   *         value: 0.0000000000001234 (1.234e-13)
   *         toString() => "1.234e-13"
   *         numberToStringWithoutExponent() => "0.0000000000001234"
   *     Big numbers:
   *         value: 1234123412341230000000 (1.234123412341234e+21)
   *         toString() => "1.234123412341234e+21"
   *         numberToStringWithoutExponent() => "1234123412341230000000"
   */
  const numberToNonExponentialString = (value) => {
      const valueAsString = (value || 0).toString();
      const valueAsStringHasExponentialInfo = valueAsString.indexOf('e') >= 0;
      if (!valueAsStringHasExponentialInfo)
          return valueAsString;
      // If the toString returns an exponential number (e.g. 1.23e+28)
      const [significand, exponent] = splitStringInTwoParts$1(valueAsString, 'e');
      const exponentAsNumber = +exponent;
      return exponentAsNumber >= 0
          ? formatPositiveExponentResult(valueAsString)
          : formatNegativeExponentResult(value, exponentAsNumber, significand);
  };

  var numberToNonExponentialString$1 = numberToNonExponentialString;

  const addTrailingZerosInFractionalPart = (valueAsString, minimumFractionDigits) => {
      const [integerPart, fractionalPart] = splitStringInTwoParts$1(valueAsString, '.');
      return `${integerPart}.${fractionalPart + stringRepeat$1('0', minimumFractionDigits - fractionalPart.length)}`;
  };
  /**
   * Implementation of Number.prototype.toFixed() that treats floats more like decimals
   *
   * Fixes binary rounding issues (eg. (0.615).toFixed(2) === '0.61') that present
   * problems for accounting- and finance-related software.
   *
   * <!> This function should only receive a finite number, never NaN, Infinity or -Infinity
   * <i> This function should return always a JS string representation of a number, but without exponent.
   * <i> optionalFractionDigits means: from the fractionDigits amount, the ones that are optional.
   */
  const numberToFixed = (finiteNumber, fractionDigits, roundingFunction, optionalFractionDigits) => {
      const valueAsString = numberToNonExponentialString$1(finiteNumber);
      const minimumFractionDigits = fractionDigits - (optionalFractionDigits || 0);
      const fractionalPartOfValueAsString = splitStringInTwoParts$1(valueAsString, '.')[1];
      const targetFractionDigitsAmount = !!fractionalPartOfValueAsString
          ? Math.min(Math.max(fractionalPartOfValueAsString.length, minimumFractionDigits), fractionDigits)
          : minimumFractionDigits;
      const roundedValue = roundNumber$1(finiteNumber, targetFractionDigitsAmount, roundingFunction);
      let output = numberToNonExponentialString$1(roundedValue);
      // Add trailing zeros if needed
      if (!!minimumFractionDigits) {
          output = addTrailingZerosInFractionalPart(output, minimumFractionDigits);
      }
      return output;
  };

  var numberToFixed$1 = numberToFixed;

  const roundValueAndAddTrailingZeros = (value, patternRules, options) => {
      const { rounding } = options;
      const { minimumFractionDigits, maximumFractionDigits } = patternRules;
      const resolvedRoundingFunction = rounding || Math.round;
      const shouldIncludeDecimalPlaces = minimumFractionDigits > 0 || maximumFractionDigits > 0;
      if (shouldIncludeDecimalPlaces) {
          const optionalDecimalDigitsCount = maximumFractionDigits - minimumFractionDigits;
          return numberToFixed$1(value, maximumFractionDigits, resolvedRoundingFunction, optionalDecimalDigitsCount);
      }
      else {
          return numberToFixed$1(value, 0, resolvedRoundingFunction);
      }
  };

  var roundValueAndAddTrailingZeros$1 = roundValueAndAddTrailingZeros;

  const replaceDigitsWithNumeralSystem = (numericString, numeralSystemMap) => {
      if (!numeralSystemMap || numeralSystemMap.length !== 10)
          return numericString;
      const numericStringLength = numericString.length;
      let output = '';
      for (let numericStringIndex = 0; numericStringIndex < numericStringLength; numericStringIndex++) {
          const char = numericString[numericStringIndex];
          output += numeralSystemMap[char] || char;
      }
      return output;
  };

  var replaceDigitsWithNumeralSystem$1 = replaceDigitsWithNumeralSystem;

  /**
   * <i> Add or remove leading zeros
   * Expects: a value integer part as string, without the minus sign. And nothing else but a number at the start.
   * Returns:
   *     - The value with the added or removed leading zeros
   */
  const addOrRemoveLeadingZerosToValue = (valueIntegerPartWithoutSign, patternRules) => {
      const { minimumIntegerDigits } = patternRules;
      if (minimumIntegerDigits === 0 && +valueIntegerPartWithoutSign < 1 && +valueIntegerPartWithoutSign > -1) {
          return '';
      }
      return valueIntegerPartWithoutSign.length >= minimumIntegerDigits
          ? valueIntegerPartWithoutSign
          : `${stringRepeat$1('0', minimumIntegerDigits - valueIntegerPartWithoutSign.length)}${valueIntegerPartWithoutSign}`;
  };

  var addOrRemoveLeadingZerosToValue$1 = addOrRemoveLeadingZerosToValue;

  const addSignInfoToFullFormattedNumber = (fullFormattedValueWithoutSign, isValueNegative, isValueZero, patternRules) => {
      const { negativeParentheses, forceSign } = patternRules;
      let output = fullFormattedValueWithoutSign;
      if (negativeParentheses && isValueNegative) {
          output = output.replace(/'ɵ(nps|npe)'/g, match => match === `'ɵnps'` ? '(' : ')');
      }
      else if (forceSign) {
          output = output.replace(`'ɵs'`, isValueNegative ? '-' : isValueZero ? '' : '+');
      }
      else if (isValueNegative) {
          output = output.replace(`'ɵs'`, '-');
      }
      return output;
  };

  var addSignInfoToFullFormattedNumber$1 = addSignInfoToFullFormattedNumber;

  /**
   * Splits the given number (as string) in the integer and decimal parts.
   * Returns:
   *     [integerPart: string, decimalPart: string]
   * <i> The integer part can potentially contain the number sign (-) if it wasn't removed previously.
   * <i> It should always return [string, string]
   */
  const splitNumberIntegerAndDecimalParts = (valueAsString, patternRules) => {
      const { optionalFractionDigits } = patternRules;
      const [integerPart, decimalPart] = splitStringInTwoParts$1(valueAsString, '.');
      // Checks whether optionalDecimalPlaces [.] is enabled and the value is an integer (no decimals)
      if (optionalFractionDigits && Number(decimalPart) === 0) {
          return [integerPart, ''];
      }
      return [integerPart, decimalPart];
  };

  var splitNumberIntegerAndDecimalParts$1 = splitNumberIntegerAndDecimalParts;

  const addThousandsSeparatorToValueIntegerPart = (valueIntegerPartWithLeadingZerosAndWithoutSign, patternRules, options) => {
      const { delimiters, digitGroupingStyle } = options.locale;
      const { grouping } = patternRules;
      if (!grouping || !delimiters.thousands) {
          return valueIntegerPartWithLeadingZerosAndWithoutSign;
      }
      const valueAsString = valueIntegerPartWithLeadingZerosAndWithoutSign;
      const thousandsSeparator = delimiters.thousands;
      const digitGrouping = !!(digitGroupingStyle === null || digitGroupingStyle === void 0 ? void 0 : digitGroupingStyle.length) ? digitGroupingStyle : [3];
      const restDigitGrouping = [...digitGrouping];
      let output = '';
      let groupingSubIteration = 1;
      for (let i = valueAsString.length - 1; i >= 0; i--) {
          if (groupingSubIteration === restDigitGrouping[0] && i !== 0) {
              output = thousandsSeparator + valueAsString[i] + output;
              if (restDigitGrouping.length > 1)
                  restDigitGrouping.shift();
              groupingSubIteration = 1;
          }
          else {
              output = valueAsString[i] + output;
              groupingSubIteration += 1;
          }
      }
      return output;
  };

  var addThousandsSeparatorToValueIntegerPart$1 = addThousandsSeparatorToValueIntegerPart;

  /**
   * Applies the localized abbreviation unit to the pattern mask
   * <i> If the localized unit is empty (''), it will remove the space between the number and the abbreviation.
   * <i> Replaces the single quotes from the abbreviation, to prevent collision with patternMask escaped text.
   */
  const applyAbbreviationLocalizedUnitToPatternMask = (patternMask, abbreviationLocalizedUnit, hasAbbreviationInPatternMask) => {
      if (!hasAbbreviationInPatternMask)
          return patternMask;
      if (abbreviationLocalizedUnit) {
          /**
           * If it has abbreviation in the rules, and has a valid unit (e.g. K | M | B | T, or
           * other localized one), escapes the single quotes in the localized abbreviation unit and appends to the mask.
           */
          return patternMask.replace(`'ɵa'`, _ => `'${abbreviationLocalizedUnit.replace(/'/g, _ => "\\'")}'`);
      }
      else {
          // If it has abbreviation in the rules, but it has no unit, removes the space between abbreviation and number
          return patternMask.match(/'ɵn'\s*'ɵa'/)
              // If abbreviation is before
              ? patternMask.replace(/\s*'ɵa'/, '')
              // If abbreviation is after
              : patternMask.replace(/'ɵa'\s*/, '');
      }
  };

  var applyAbbreviationLocalizedUnitToPatternMask$1 = applyAbbreviationLocalizedUnitToPatternMask;

  const scaleAndRoundValue = (number, patternRules, options) => {
      // If it doesn't have abbreviation, just round the value and add trailing zeros
      if (!patternRules.compact) {
          const roundedValueAsString = roundValueAndAddTrailingZeros$1(number, patternRules, options);
          return [roundedValueAsString, null];
      }
      // If it has abbreviation, scales the value
      const [scaledValue, scaledValueLocalizedUnit] = scaleValueWithAbbreviation$1(number, patternRules, options);
      const roundedScaledValue = +roundValueAndAddTrailingZeros$1(scaledValue, patternRules, options);
      const [rescaledValue, rescaledValueLocalizedUnit] = rescaleRoundedValue$1(+roundedScaledValue, scaledValueLocalizedUnit, patternRules, options);
      const roundedRescaledValueAsStringWithTrailingZeros = roundValueAndAddTrailingZeros$1(rescaledValue, patternRules, options);
      return [roundedRescaledValueAsStringWithTrailingZeros, rescaledValueLocalizedUnit];
  };
  const numberToFormattedNumber = (number, pattern, options) => {
      var _a;
      const patternRules = parsePattern$1(pattern);
      // Ensure always uses a number or default number
      const resolvedValue = isFiniteNumber$1(number) ? number : 0;
      const [valueAsString, localizedAbbreviationUnit] = scaleAndRoundValue(resolvedValue, patternRules, options);
      // Prevents potentially wrong formatting coming from this function
      if (valueAsString === 'NaN')
          return '';
      const isValueNegative = options.signedZero ? number < 0 : +valueAsString < 0;
      const isValueZero = options.signedZero ? number === 0 : +valueAsString === 0;
      const valueAsStringWithoutSign = removeSignIfExists$1(valueAsString);
      const [integerPart, decimalPart] = splitNumberIntegerAndDecimalParts$1(valueAsStringWithoutSign, patternRules);
      const valueIntegerPartWithLeadingZeros = addOrRemoveLeadingZerosToValue$1(integerPart, patternRules);
      const valueIntegerPartWithThousandsSeparator = addThousandsSeparatorToValueIntegerPart$1(valueIntegerPartWithLeadingZeros, patternRules, options);
      const numeralSystemFromLocale = options.locale.numeralSystem;
      const integerPartWithNumeralSystem = replaceDigitsWithNumeralSystem$1(valueIntegerPartWithThousandsSeparator, numeralSystemFromLocale);
      const decimalPartWithNumeralSystem = replaceDigitsWithNumeralSystem$1(decimalPart, numeralSystemFromLocale);
      const fullNumberWithNumeralSystem = (integerPartWithNumeralSystem
          + (!!decimalPartWithNumeralSystem ? (((_a = options.locale.delimiters) === null || _a === void 0 ? void 0 : _a.decimal) || '.') + decimalPartWithNumeralSystem : ''));
      // Assembling
      const patternMaskWithAbbreviation = applyAbbreviationLocalizedUnitToPatternMask$1(patternRules.patternMask, localizedAbbreviationUnit, patternRules.compact);
      const patternMaskWithNumber = patternMaskWithAbbreviation.replace(`'ɵn'`, _ => `'${fullNumberWithNumeralSystem.replace(/'/g, "\\'")}'`);
      const patternMaskWithSignInfo = addSignInfoToFullFormattedNumber$1(patternMaskWithNumber, isValueNegative, isValueZero, patternRules);
      const cleanPatternMask = patternMaskWithSignInfo.replace(/'ɵ(nps|npe|s|a|n)'/g, '');
      const fullFormattedValueWithNormalizedText = patternStripAndNormalizeEscapedText(cleanPatternMask);
      return fullFormattedValueWithNormalizedText;
  };

  var numberToFormattedNumber$1 = numberToFormattedNumber;

  /**
   * Basis point format (BPS)
   * <i> See https://en.wikipedia.org/wiki/Basis_point
   */
  const bpsFormatter = {
      name: 'bps',
      regexps: {
          format: /BPS/,
          unformat: /BPS/,
      },
      format: (number, pattern, options) => {
          const scaledValue = multiplyByPowerOfTen$1(number, 4);
          const patternWithEscapedBPS = patternReplace(pattern, /BPS/, `'ɵBPSɵ'`);
          const formatResult = numberToFormattedNumber$1(scaledValue, patternWithEscapedBPS, options);
          return formatResult.replace('ɵBPSɵ', 'BPS');
      },
      unformat: (string, options) => {
          const number = formattedStringToNumber$1(string.replace(/\s?BPS/, ''), options);
          return isFiniteNumber$1(number) ? multiplyByPowerOfTen$1(number, -4) : number;
      },
  };

  var bpsFormatter$1 = bpsFormatter;

  const timeFormatter = {
      name: 'time',
      regexps: {
          format: /([0-9]{1,2}:[0-9]{2}) *$/,
          unformat: /([0-9]{1,2}:[0-9]{2}) *$/,
      },
      format: (number) => {
          const absoluteValue = Math.abs(number);
          const sign = number < 0 ? '-' : '';
          const hours = truncateNumber$1(absoluteValue / 3600);
          const minutes = truncateNumber$1((absoluteValue - (hours * 3600)) / 60);
          const seconds = truncateNumber$1(absoluteValue - (hours * 3600) - (minutes * 60));
          return `${sign}${hours}:${(minutes < 10 ? '0' : '') + minutes}:${(seconds < 10 ? '0' : '') + seconds}`;
      },
      unformat: (string) => {
          const isNegative = /^ *-/.test(string);
          const stringWithoutSign = string.replace(/^ *-/, '');
          const timeArray = stringWithoutSign.split(':').reverse();
          let seconds = 0;
          seconds += +timeArray[0];
          seconds += +timeArray[1] * 60;
          seconds += (+timeArray[2] || 0) * 3600;
          return isNegative && seconds !== 0 ? -seconds : seconds;
      },
  };

  var timeFormatter$1 = timeFormatter;

  const decimalSuffixes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];
  const binarySuffixes = ['B', 'KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB'];
  const allSuffixes = decimalSuffixes.concat(binarySuffixes.slice(1));
  /** Avoid collision with BPS format @see formats|bps.ts */
  const unformatRegex = `(${allSuffixes.join('|').replace(/B/g, 'B(?!PS)')})`;
  const bytesDecimalScale = unitScale({ base: 'B', scale: toObject$1(decimalSuffixes, (unit, unitIndex) => [unit, Math.pow(1000, unitIndex)]) });
  const bytesBinaryScale = unitScale({ base: 'B', scale: toObject$1(binarySuffixes, (unit, unitIndex) => [unit, Math.pow(1024, unitIndex)]) });
  const bytesFormatter = {
      name: 'bytes',
      regexps: {
          format: /([0\s]b[bd])|(b[bd][0\s])/,
          unformat: (string, options) => options.type === 'bytes' ? new RegExp(unformatRegex).test(string) : false,
      },
      format: (number, pattern, options) => {
          const scale = patternIncludes(pattern, 'bb') ? bytesBinaryScale : bytesDecimalScale;
          const [scaledValue, scaledValueUnit] = scale.toBest(number, 'B');
          const patternWithEscapedBytes = patternReplace(pattern, /b[bd]/, `'ɵbytesɵ'`);
          const formatResult = numberToFormattedNumber$1(scaledValue, patternWithEscapedBytes, options);
          return formatResult.replace('ɵbytesɵ', scaledValueUnit || '');
      },
      unformat: (string, options) => {
          var _a;
          const number = formattedStringToNumber$1(string.replace(new RegExp(unformatRegex), ''), options);
          const suffix = ((_a = string.match(unformatRegex)) === null || _a === void 0 ? void 0 : _a[0]) || '';
          const scale = !!bytesBinaryScale.scaleDefinition.scale[suffix] ? bytesBinaryScale : bytesDecimalScale;
          return number ? scale.toBase(number, suffix) : number;
      }
  };

  var bytesFormatter$1 = bytesFormatter;

  const ordinalFormatter = {
      name: 'ordinal',
      regexps: {
          format: /o/,
      },
      format: (number, pattern, options) => {
          var _a, _b;
          const localizedOrdinal = ((_b = (_a = options.locale).ordinal) === null || _b === void 0 ? void 0 : _b.call(_a, number)) || '';
          const patternWithEscapedOrdinal = patternReplace(pattern, /o/, `'ɵordɵ'`);
          const formatResult = numberToFormattedNumber$1(number, patternWithEscapedOrdinal, options);
          return formatResult.replace('ɵordɵ', _ => localizedOrdinal);
      }
  };

  var ordinalFormatter$1 = ordinalFormatter;

  const currencySymbolsMap = {
      EUR: '€',
      USD: '$',
      XCD: 'EC$',
      AUD: 'A$',
      INR: '₹',
      BRL: 'R$',
      CAD: 'CA$',
      XAF: 'FCFA',
      CNY: 'CN¥',
      NZD: 'NZ$',
      XPF: 'CFPF',
      GBP: '£',
      HKD: 'HK$',
      ILS: '₪',
      JPY: '¥',
      KRW: '₩',
      XOF: 'CFA',
      MXN: 'MX$',
      TWD: 'NT$',
      VND: '₫',
  };
  const currencyFormatter = {
      name: 'currency',
      regexps: {
          format: /(\$)/,
      },
      format: (number, pattern, options) => {
          var _a;
          const currencyFromOptions = (_a = options.currency) === null || _a === void 0 ? void 0 : _a.toUpperCase();
          const localizedCurrencySymbol = currencySymbolsMap[currencyFromOptions] || currencyFromOptions || '';
          const patternWithEscapedCurrencySymbol = patternReplace(pattern, /\$/, `'ɵcurrencyɵ'`);
          const formatResult = numberToFormattedNumber$1(number, patternWithEscapedCurrencySymbol, options);
          return formatResult.replace('ɵcurrencyɵ', _ => localizedCurrencySymbol);
      },
  };

  var currencyFormatter$1 = currencyFormatter;

  const percentageFormatter = {
      name: 'percentage',
      regexps: {
          format: /%!?/,
          unformat: /%/,
      },
      format: (number, pattern, options) => {
          const hasNotScalePercentageSymbolInPattern = patternIncludes(pattern, '%!');
          const scaledValue = options.scalePercentage && !hasNotScalePercentageSymbolInPattern ? multiplyByPowerOfTen$1(number, 2) : number;
          const patternWithEscapedPercentage = patternReplace(pattern, /%!?/, `'ɵ%ɵ'`);
          const formatResult = numberToFormattedNumber$1(scaledValue, patternWithEscapedPercentage, options);
          return formatResult.replace('ɵ%ɵ', '%');
      },
      unformat: (string, options) => {
          const number = formattedStringToNumber$1(string.replace(/\s?%/, ''), options);
          return number && options.scalePercentage ? multiplyByPowerOfTen$1(number, -2) : number;
      },
  };

  var percentageFormatter$1 = percentageFormatter;

  const exponentialFormatter = {
      name: 'exponential',
      regexps: {
          format: /[eE][+-][0-9]+/,
          unformat: /[eE][+-][0-9]+/,
      },
      format: (number, pattern, options) => {
          const exponential = typeof number === 'number' && !isNaNNumber$1(number) ? number.toExponential() : '0e+0';
          const parts = splitStringInTwoParts$1(exponential, 'e');
          const patternWithoutExponential = patternReplace(pattern, /e[+|-]{1}0/i, '');
          const formatResult = numberToFormattedNumber$1(+parts[0], patternWithoutExponential, options);
          return formatResult + 'e' + parts[1];
      },
      unformat: (string, options) => {
          var _a;
          const value = formattedStringToNumber$1(string.replace(/e[+-]{1}[0-9]{1,3}/i, ''), options);
          const powerOfTenExponent = +(((_a = string.match(/e([+-]{1}[0-9]{1,3})/i)) === null || _a === void 0 ? void 0 : _a[1]) || '0');
          return isFiniteNumber$1(value) ? multiplyByPowerOfTen$1(value, powerOfTenExponent) : value;
      },
  };

  var exponentialFormatter$1 = exponentialFormatter;

  const BUILT_IN_FORMATTERS = [
      percentageFormatter$1,
      currencyFormatter$1,
      ordinalFormatter$1,
      timeFormatter$1,
      bytesFormatter$1,
      exponentialFormatter$1,
      bpsFormatter$1,
  ];

  var BUILT_IN_FORMATTERS$1 = BUILT_IN_FORMATTERS;

  const roundHalfAwayFromZero = (value) => {
      return value >= 0
          ? Math.round(value)
          : (value % 0.5 === 0) ? Math.floor(value) : Math.round(value);
  };

  var roundHalfAwayFromZero$1 = roundHalfAwayFromZero;

  const areDelimitersValid = (delimiters) => {
      return !!(delimiters === null || delimiters === void 0 ? void 0 : delimiters.decimal)
          && isString$1(delimiters === null || delimiters === void 0 ? void 0 : delimiters.thousands)
          && delimiters.decimal !== delimiters.thousands;
  };
  const resolveOptionsLocale = (optionsLocale) => {
      const defaultLocale = locale$1;
      if (!isObject$1(optionsLocale))
          return defaultLocale;
      return merge(optionsLocale, {
          delimiters: areDelimitersValid(optionsLocale.delimiters) ? optionsLocale.delimiters : defaultLocale.delimiters,
          abbreviations: optionsLocale.abbreviations || defaultLocale.abbreviations,
          ordinal: optionsLocale.ordinal || defaultLocale.ordinal,
      });
  };
  const resolveRoundingOption = (roundingOption) => {
      switch (roundingOption) {
          case 'ceil': return Math.ceil;
          case 'floor': return Math.floor;
          case 'truncate': return truncateNumber$1;
          case 'half-up': return Math.round;
          case 'half-away-from-zero': return roundHalfAwayFromZero$1;
          default: return isFunction$1(roundingOption) ? roundingOption : roundHalfAwayFromZero$1;
      }
  };
  const resolveOptionsFormatters = (optionsFormatters) => {
      if (!optionsFormatters)
          return BUILT_IN_FORMATTERS$1;
      return isFunction$1(optionsFormatters)
          ? optionsFormatters(BUILT_IN_FORMATTERS$1)
          : [...optionsFormatters, ...BUILT_IN_FORMATTERS$1];
  };
  const resolveFormatOptions = (formatOptions) => {
      var _a, _b, _c;
      const options = formatOptions || {};
      const resolvedRoundingFunction = resolveRoundingOption(options.rounding);
      const resolvedLocale = resolveOptionsLocale(options.locale);
      const resolvedFormatters = resolveOptionsFormatters(options.formatters);
      return {
          defaultPattern: options.defaultPattern || '0,0.##########',
          nullFormat: options.nullFormat || '',
          nanFormat: options.nanFormat,
          zeroFormat: options.zeroFormat,
          locale: resolvedLocale,
          rounding: resolvedRoundingFunction,
          type: options.type,
          scalePercentage: (_a = options.scalePercentage) !== null && _a !== void 0 ? _a : true,
          trim: (_b = options.trim) !== null && _b !== void 0 ? _b : true,
          formatters: resolvedFormatters,
          currency: options.currency,
          signedZero: !!options.signedZero,
          nonBreakingSpace: (_c = options.nonBreakingSpace) !== null && _c !== void 0 ? _c : false,
      };
  };

  var resolveFormatOptions$1 = resolveFormatOptions;

  const getUnformatFunctionIfMatch = (input, resolvedOptions) => {
      for (const formatter of resolvedOptions.formatters) {
          const matcher = formatter.regexps.unformat;
          if (!matcher)
              continue;
          const matcherResult = isFunction$1(matcher) ? matcher(input, resolvedOptions) : !!input.match(matcher);
          if (matcherResult)
              return formatter.unformat;
      }
  };
  const parse$2 = (input, options) => {
      const resolvedOptions = resolveFormatOptions$1(options);
      let value;
      if (isNil$1(input) || isNaNNumber$1(input)) {
          value = null;
      }
      else if (typeof input === 'number') {
          // Handles negative zero
          value = input === 0 ? 0 : input;
      }
      else if (typeof input === 'string') {
          if (resolvedOptions.zeroFormat && input === resolvedOptions.zeroFormat) {
              value = 0;
          }
          else if (resolvedOptions.nullFormat && input === resolvedOptions.nullFormat) {
              value = null;
          }
          else {
              // Removes non-breaking spaces if they exists
              const inputStringWithNormalSpaces = input.replace(/\u00A0/, ' ');
              const unformatFunctionFromFormatters = getUnformatFunctionIfMatch(inputStringWithNormalSpaces, resolvedOptions);
              const unformatFunction = unformatFunctionFromFormatters || formattedStringToNumber$1;
              value = unformatFunction(inputStringWithNormalSpaces, resolvedOptions);
          }
      }
      else {
          const result = +input;
          value = result === 0 ? result : (result || null);
      }
      return value;
  };

  var parse$1$1 = parse$2;

  /**
   * @example
   * ```javascript
   * parse('1,250.48')
   * //=> 1250.48
   * parse('10 %')
   * //=> 0.1
   * parse('1 000,582', { locale: fr })
   * //=> 1000.582
   * ```
   * Parse the given numeric-string applying the provided options.
   *
   * options:
   * ```typescript
   * {
   * nullFormat?: string;
   * nanFormat?: string;
   * zeroFormat?: string;
   * defaultPattern?: string;
   * rounding?: 'truncate' | 'ceil' | 'floor' | 'round' | ((scaledValueForRounding: number) => number);
   * locale?: NumerableLocale;
   * type?: string;
   * scalePercentage?: boolean;
   * formatters?: NumerableFormatter[] | ((builtInFormatters: NumerableFormatter[]) => NumerableFormatter[]);
   * }
   * ```
   *
   * @param string string: The numeric-string to parse (e.g. **'10 %'**)
   * @param options options: The options used to parse the numeric-string
   * ```typescript
   * {
   * nullFormat?: string;
   * nanFormat?: string;
   * zeroFormat?: string;
   * defaultPattern?: string;
   * rounding?: 'truncate' | 'ceil' | 'floor' | 'round' | ((scaledValueForRounding: number) => number);
   * locale?: NumerableLocale;
   * type?: string;
   * scalePercentage?: boolean;
   * formatters?: NumerableFormatter[] | ((builtInFormatters: NumerableFormatter[]) => NumerableFormatter[]);
   * }
   * ```
   */
  const parse = (string, options) => {
      return parse$1$1(string, options);
  };

  var parse$1 = parse;

  /**
   * @example
   * ```javascript
   * round(12.687, 2)
   * //=> 12.69
   * round(12.687)
   * //=> 13
   * round(12.687, 2, Math.floor)
   * //=> 12.68
   * ```
   * Rounds the given number to the specified amount of decimal places.
   *
   * - The **default precision** is 0.
   * - The **default roundingFunction** is Math.round.
   *
   * @param number number: The number to round (e.g. **10.23**)
   * @param precision precision: The desired amount of decimal places (e.g. **2**)
   * @param roundingFunction roundingFunction: The function applied for rounding (e.g. **Math.floor**)
   * */
  const round = (number, precision, roundingFunction) => {
      return roundNumber$1(number, precision, roundingFunction);
  };

  var round$1 = round;

  const getFormatFunctionIfMatch = (pattern, resolvedOptions) => {
      const patternWithoutEscapedText = patternRemoveEscapedText(pattern);
      for (const formatter of resolvedOptions.formatters) {
          const matcher = formatter.regexps.format;
          if (!matcher)
              continue;
          const matcherResult = isFunction$1(matcher) ? matcher(pattern, resolvedOptions) : !!patternWithoutEscapedText.match(matcher);
          if (matcherResult)
              return formatter.format;
      }
  };
  const format$1 = (value, pattern, options) => {
      var _a;
      try {
          const resolvedValue = isString$1(value) ? parseFloat(value) : value;
          const resolvedOptions = resolveFormatOptions$1(options);
          const resolvedPattern = pattern || resolvedOptions.defaultPattern;
          let output;
          if (resolvedValue === Infinity || resolvedValue === -Infinity) {
              output = resolvedValue > 0 ? '∞' : '-∞';
          }
          else if (isNaNNumber$1(resolvedValue)) {
              return isString$1(resolvedOptions.nanFormat)
                  ? resolvedOptions.nanFormat
                  : (isString$1(resolvedOptions.nullFormat) ? resolvedOptions.nullFormat : '');
          }
          else if (isNil$1(resolvedValue)) {
              output = isString$1(resolvedOptions.nullFormat) ? resolvedOptions.nullFormat : '';
          }
          else if (resolvedValue === 0 && isString$1(resolvedOptions.zeroFormat)) {
              output = resolvedOptions.zeroFormat;
          }
          else {
              // <!> Here value should always be a number
              const resolvedValueAsNumber = resolvedValue || 0;
              const formatFunctionFromFormatters = getFormatFunctionIfMatch(resolvedPattern, resolvedOptions);
              const resolvedFormatFunction = formatFunctionFromFormatters || numberToFormattedNumber$1;
              output = resolvedFormatFunction(resolvedValueAsNumber, resolvedPattern, resolvedOptions);
          }
          // Ensures that it always returns an string
          output = isString$1(output) ? output : '';
          // Replaces spaces with non-breaking spaces if needed
          output = resolvedOptions.nonBreakingSpace
              ? output.replace(/ /g, _ => '\u00A0')
              : output;
          // Trims the output if needed
          output = resolvedOptions.trim ? output.trim() : output;
          return output;
      }
      catch (_error) {
          return ((_a = options) === null || _a === void 0 ? void 0 : _a._errorFormat) || '';
      }
  };

  var format$1$1 = format$1;

  function format(number, arg2, arg3) {
      const pattern = isString$1(arg2) ? arg2 : null;
      const options = isObject$1(arg2) ? arg2 : (isObject$1(arg3) ? arg3 : {});
      return format$1$1(number, pattern, options);
  }
  const createFormatFunction = (options) => {
      const baseOptions = merge(options, {
          locale: isFunction$1(options.locale) ? options.locale() : options.locale,
      });
      return ((value, arg2, arg3) => {
          const pattern = isString$1(arg2) ? arg2 : null;
          const optionsFromArguments = isObject$1(arg2) ? arg2 : (isObject$1(arg3) ? arg3 : {});
          return format$1$1(value, pattern, merge(baseOptions, optionsFromArguments));
      });
  };
  format.withOptions = createFormatFunction;

  const unique = (arr) => {
      if (!arr)
          return [];
      return arr.filter((value, index, self) => self.indexOf(value) === index);
  };

  var unique$1 = unique;

  // <i> Extracted from https://stackoverflow.com/questions/12006095/javascript-how-to-check-if-character-is-rtl
  const leftToRightMark = '\u200e';
  const rtlCharsRanges = '\u0591-\u07FF\u200F\u202B\u202E\uFB1D-\uFDFD\uFE70-\uFEFC';
  const rtlDirCheck = new RegExp('^[^' + rtlCharsRanges + ']*?[' + rtlCharsRanges + ']');
  const isRTL = (string) => rtlDirCheck.test(string);
  const appendLeftToRightMarkIfIsRTL = (string) => isRTL(string) ? string + leftToRightMark : string;
  // const languagesWith4DigitsGroupingStyle = ['zh', 'yue', 'ko', 'ja'];
  const languagesWith4DigitsGroupingStyle = []; //***
  const toLocaleStringSupportsOptions = () => {
      return typeof Intl === 'object' && !!Intl && typeof Intl.NumberFormat === 'function';
  };
  const getNumeralSystemDigits = (languageTag) => {
      try {
          const localizedNumber = (1234567890).toLocaleString(languageTag, { useGrouping: false });
          const lookupObject = {};
          const repeatedChar = localizedNumber.split('').find((char) => {
              if (lookupObject[char])
                  return true;
              lookupObject[char] = true;
          });
          const digitsWithoutGroupingDelimiters = repeatedChar
              ? localizedNumber.replace(new RegExp(escapeRegexString$1(repeatedChar || ''), 'g'), '')
              : localizedNumber;
          const digitsAsArray = digitsWithoutGroupingDelimiters.split('');
          const sortedDigits = [digitsAsArray[digitsAsArray.length - 1], ...digitsAsArray.slice(0, -1)];
          return sortedDigits.join('');
      }
      catch (_err) {
          return null;
      }
  };
  const getGroupingAndFractionDelimiters = (languageTag, digits) => {
      try {
          const localizedNumber = (12345678.123).toLocaleString(languageTag);
          const localizedNumberWithoutDigits = localizedNumber.replace(new RegExp(`[${escapeRegexString$1(digits)}]`, 'g'), '');
          const [groupingDelimiter = ',', fractionDelimiter = '.'] = unique$1(localizedNumberWithoutDigits.split(''));
          return [groupingDelimiter, fractionDelimiter];
      }
      catch (_err) {
          return null;
      }
  };
  const getGroupingStyle = (languageTag, groupingDelimiter) => {
      // <i> Handle '4 digits' grouping style for some asian countries (not CLDR)
      if (languagesWith4DigitsGroupingStyle.some(language => languageTag.indexOf(language) === 0))
          return [4];
      try {
          const result = [];
          let subIterationIndex = 0;
          (100000000000).toLocaleString(languageTag).split('').reverse().forEach((digitOrGroupingDelimiter) => {
              if (digitOrGroupingDelimiter === groupingDelimiter) {
                  result.push(subIterationIndex);
                  subIterationIndex = 0;
              }
              else {
                  subIterationIndex += 1;
              }
          });
          let resultIndex = result.length;
          while (resultIndex--) {
              if (result[resultIndex] === result[resultIndex - 1])
                  result.pop();
              else
                  break;
          }
          return result;
      }
      catch (_err) {
          return null;
      }
  };
  const getAbbreviations = (languageTag, digits, type) => {
      try {
          if (!toLocaleStringSupportsOptions())
              return null;
          const intlFormatOptions = { notation: 'compact', useGrouping: false, compactDisplay: type };
          const [digitOfZero, digitOfOne, digitOfTwo] = digits.split('');
          let abbreviations = '';
          for (let i = 1; i < 50; i++) {
              const abbreviationResultForOne = (+(1 + stringRepeat$1('0', i))).toLocaleString(languageTag, intlFormatOptions);
              if (new RegExp(`^${digitOfOne}[^${digitOfZero}]+$`).test(abbreviationResultForOne)) {
                  if (type === 'long') {
                      const abbreviationResultForTwo = (+(2 + stringRepeat$1('0', i))).toLocaleString(languageTag, intlFormatOptions);
                      const abbreviationOne = abbreviationResultForOne.replace(new RegExp(`${digitOfOne}`, 'g'), '').trim();
                      const abbreviationTwo = abbreviationResultForTwo.replace(new RegExp(`${digitOfTwo}`, 'g'), '').trim();
                      abbreviations += '|' + appendLeftToRightMarkIfIsRTL(abbreviationOne) + ':::' + appendLeftToRightMarkIfIsRTL(abbreviationTwo);
                  }
                  else {
                      const abbreviation = abbreviationResultForOne.replace(new RegExp(`${digitOfOne}`, 'g'), '').trim();
                      abbreviations += '|' + appendLeftToRightMarkIfIsRTL(abbreviation);
                  }
              }
              else {
                  abbreviations += '|';
              }
          }
          // Remove trailing pipes '|'
          let result = abbreviations;
          let resultIndex = result.length;
          while (resultIndex--) {
              if (result[resultIndex] === '|')
                  result = result.slice(0, -1);
              else
                  break;
          }
          return result;
      }
      catch (_err) {
          return null;
      }
  };
  const baseGetLocaleFromPlatform = (languageTag) => {
      const resolvedLanguageTag = languageTag || 'en';
      const digits = getNumeralSystemDigits(resolvedLanguageTag);
      const resolvedDigits = digits || '0123456789';
      const delimiters = getGroupingAndFractionDelimiters(resolvedLanguageTag, resolvedDigits);
      const [groupingDelimiter, fractionDelimiter] = !!delimiters && delimiters.length >= 2 ? delimiters : [',', '.'];
      const groupingStyle = getGroupingStyle(resolvedLanguageTag, groupingDelimiter);
      const shortAbbreviations = getAbbreviations(resolvedLanguageTag, resolvedDigits, 'short');
      const longAbbreviations = getAbbreviations(resolvedLanguageTag, resolvedDigits, 'long');
      return {
          _abbreviationsLong: longAbbreviations || locale$1.abbreviations,
          code: resolvedLanguageTag,
          delimiters: { thousands: groupingDelimiter, decimal: fractionDelimiter },
          abbreviations: shortAbbreviations || locale$1.abbreviations,
          digitGroupingStyle: !!(groupingStyle === null || groupingStyle === void 0 ? void 0 : groupingStyle.length) ? groupingStyle : undefined,
          numeralSystem: digits !== '0123456789' ? digits === null || digits === void 0 ? void 0 : digits.split('').map(appendLeftToRightMarkIfIsRTL) : undefined,
          ordinal: locale$1.ordinal,
      };
  };
  const getLocaleFromPlatform$2 = memoize$1(baseGetLocaleFromPlatform);

  var getLocaleFromPlatform$1$1 = getLocaleFromPlatform$2;

  /**
   * Given a language tag (e.g. '**zh**' | '**es**' | '**fr**' | '**en-IN**' | '**zh-Hans**'), returns a NumerableLocale
   * object extracted from the platform Intl.NumberFormat behavior.
   *
   * This locale object can be used in the numerable functions that support i18n (*format* and *parse*).
   * Example:
   * ```javascript
   * format(12345, '0,0.00', { locale: getLocaleFromPlatform('fr') })
   * ```
   *
   * <i> Take into account that the returned locale is not complete, and some features like
   *     'ordinal formatting' won't work. Use this feature only for simple applications that don't require
   *     full support from numeral, and don't target legacy browsers.
   */
  const getLocaleFromPlatform = (languageTag) => {
      return getLocaleFromPlatform$1$1(languageTag);
  };

  var getLocaleFromPlatform$1 = getLocaleFromPlatform;

  var numerable = {
    __proto__: null,
    parse: parse$1,
    round: round$1,
    format,
    getLocaleFromPlatform: getLocaleFromPlatform$1
  };

  // Extend
  Object.assign(ew__default.default, numerable);

})(ew, jQuery, luxon);
