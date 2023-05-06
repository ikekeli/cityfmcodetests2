// IDs
let ids = priv.ids;

// Get records
function getRecords() {
    let rows = priv.permissions.slice(0);
    return rows.map(row => Object.fromEntries([
        ["table", row.table], // Table caption
        ["name", row.name], // Table name
        ["index", row.index],
        ["allowed", row.allowed],
        ...ids.map(id => [id, row.permission & priv[id]])
    ]));
}

// Get formatter function
function getFormatter(id) {
    return function(cell, formatterParams) {
        let row = cell.getRow(),
            index = row.getIndex(),
            name = id + '_' + index,
            trueValue = priv[id],
            allowed = row.getData().allowed,
            checked = cell.getValue() == trueValue;
        row.checked = checked;
        return '<div class="form-check"><input type="checkbox" class="form-check-input ew-priv ew-multi-select" name="' + name + '" id="' + name +
            '" value="' + trueValue + '" data-index="' + index + '"' +
            (checked ? ' checked' : '') +
            (((allowed & trueValue) != trueValue) ? ' disabled' : '') + '></div>';
    };
}

// Get display table name
function displayTableName(cell, formatterParams) {
    let row = cell.getRow();
    return `<span data-bs-toggle="tooltip" data-bs-title="${ew.htmlEncode(row.getData().name)}">${row.getData().table}</span><input type="hidden" name="table_${row.getIndex()}" value="1">`;
}

// Get title HTML
function getTitleHtml(id, phraseId) {
    return '<div class="form-check"><input type="checkbox" class="form-check-input ew-priv" name="' + id + '" id="' + id + '" data-ew-action="select-all">' +
        '<label class="form-check-label" for="' + id + '">' + ew.language.phrase("Permission" + (phraseId || id)) + '</label></div>'
}

// Get columns
function getColumns() {
    return [{
            title: '<span class="fw-normal">' + ew.language.phrase("Tables") + '</span>',
            field: "table",
            formatter: displayTableName,
            sorter: "string",
            headerSortTristate: headerSortTristate,
            resizable: false
        },
        ...ids.map(id => {
            return {
                title: getTitleHtml(id),
                field: id,
                formatter: getFormatter(id),
                headerSort: false,
                resizable: false
            };
        })
    ];
}

// Init
($ => {
    let options = ew.deepAssign({
        index: "index",
        data: getRecords(), // Load row data from array
        layout: "fitDataFill", // Fit columns to Data
        initialSort: [ // Set the initial sort order of the data
            { column: "table", dir: "asc" },
        ],
        columnHeaderSortMulti: false, // Multi Column Sorting
        columns: getColumns() // Define the table columns
    }, tableOptions);

    let table = new Tabulator(".ew-card.ew-user-priv .ew-card-body", options);
    table.on("dataProcessed", () => {
        $("input[type=checkbox]").on("click", function() {
            let index = parseInt(this.dataset.index, 10),
                value = parseInt(this.value, 10);
            if (!isNaN(index) && !isNaN(value)) {
                if (this.checked)
                    priv.permissions[index].permission |= value;
                else
                    priv.permissions[index].permission ^= priv.permissions[index].permission ^ value;
            }
        });
        let container = document.querySelector("main");
        $("span[data-bs-toggle=tooltip][data-bs-title]").tooltip({ container, offset: [0, 4] });
        ew.initMultiSelectCheckboxes();
        ew.fixLayoutHeight();
        // console.log("dataProcessed");
    });

    // Re-load records on search
    let timer;
    $("#table-name").on("keydown keypress cut paste", () => {
        timer?.cancel();
        timer = $.later(200, null, () => table.setFilter("table", "like", $("#table-name").val()));
    });
})(jQuery);
