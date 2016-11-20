function contentListViewModel(contents) {
    var _this = this;
    this.contents = ko.observableArray();
    this.textoPesquisa = ko.observable();


    for (var i = 0; i < contents.length; i++) {
        var item = contents[i];
        item.IsVisible = ko.observable(item.IsVisible)
        _this.contents.push(item)
    }

    this.pesquisar = function () {
        var contents = _this.contents();
        for (var i = 0; i < contents.length; i++) {
            var content = contents[i];
            content.IsVisible(_this.textoPesquisa() == '' || _this.textoPesquisa() == undefined || content.Name.toLowerCase().indexOf(_this.textoPesquisa().toLowerCase()) != -1);
        }
        //_this.contents.valueHasMutated();
    }

}