var deleteActionUrl = '';
var itemId = 0;
$(document).ready(function () {

    bindBtnDelAssert();

    $(".cds-btn-del-asserted").click(function () {
        var form = document.createElement("form");
        form.action = deleteActionUrl;
        form.method = "POST";
        
        var input = document.createElement("input");
        input.name = "Id";
        input.value = itemId;

        form.appendChild(input);
        document.body.appendChild(form);
        form.submit();
        
    });
});

function bindBtnDelAssert() {
    $(".cds-btn-del-assert").unbind("click");

    $(".cds-btn-del-assert").click(function () {
        itemId = $(this).attr("data-id");
        $('#modalDelete').modal();
    });
}