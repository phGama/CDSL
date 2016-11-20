function groupSelectViewModel(groups,selectedIds) {
    var _this = this;
    this.unselectedGroups = ko.observableArray(groups);
    this.selectedGroups = ko.observableArray();
    this.selectedIds = ko.observableArray();
    this.optionSelectedGroup = ko.observable();

    this.groupIds = ko.computed(function () {
        var stringReturn = "";
        var LastIndex = _this.selectedIds().length -1;
        for (var i = 0; i <= LastIndex; i++) {
            stringReturn += _this.selectedIds()[i];
            if (i != LastIndex)
                stringReturn += ",";
        }
        return stringReturn;
    },this);

    this.add = function () {
        _this.selectedGroups.push(_this.optionSelectedGroup());
        _this.selectedIds.push(_this.optionSelectedGroup().Id);
        _this.unselectedGroups.remove(_this.optionSelectedGroup());

    }

    this.remove = function () {
        _this.selectedGroups.remove(this);
        _this.selectedIds.remove(this.Id);
        _this.unselectedGroups.push(this);
    }


    if (selectedIds !== undefined) {
        for (var i = 0; i < selectedIds.length; i++) {
            for (var j = 0; j < this.unselectedGroups().length; j++) {
                var Group = this.unselectedGroups()[j];
                if (Group.Id == selectedIds[i]) {
                    this.selectedGroups.push(Group);
                    this.selectedIds.push(Group.Id);
                    this.unselectedGroups.remove(Group);
                }
            }
        }
    }
}