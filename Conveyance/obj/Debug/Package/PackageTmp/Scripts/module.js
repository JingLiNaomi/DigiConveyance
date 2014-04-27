// Model
function Module(data) {
    var self = this;
    data = data || {};
    self.ModuleID = data.ModuleID;
    self.Type = ko.observable(data.Type);
    self.TextS = ko.observable(data.TextS || "");
    self.TextC = ko.observable(data.TextC || "");
    self.InitiateParty = ko.observable(data.InitiateParty || "");
    self.ModuleSetID = data.ModuleSetID;
    self.Communication = ko.observable(data.Communication || "");
    self.Stage = ko.observable(data.Stage || "");
    //visibility of textC
    self.vtextC = ko.computed(function () {
        return self.Type() ==1;
    }, self);
    //visibility of communication options
    self.vBuyerToSolicitor = ko.computed(function () {
        return (self.Type() == 1) && (self.InitiateParty()=="Buy");
    }, self);
    self.vSellertoSolicitor = ko.computed(function () {
        return (self.Type() == 1) && (self.InitiateParty() == "Sell");
    }, self);
    self.vSolicitorToBuyer = ko.computed(function () {
        return (self.InitiateParty() == "Buy");
    }, self);
    self.vSolicitorToSeller = ko.computed(function () {
        return (self.InitiateParty() == "Sell");
    }, self);
    self.vSSolicitorToBSolicitor = ko.computed(function () {
        return (self.Type() == 1) && (self.InitiateParty() == "Sell");
    }, self);
    self.vBSolicitorToSSolicitor = ko.computed(function () {
        return (self.Type() == 1) && (self.InitiateParty() == "Buy");
    }, self);
}






function viewModel() {
    var self = this;
    self.modules = ko.observableArray();
    self.loadModules = function () {
        $.post('/Admin/LoadModules',
            {
                path: $('#path').text()
            },
            function (data) {
                var mappedModules = $.map(data, function (item) { return new Module(item); });
                self.modules(mappedModules);
            }, 'JSON');
    }
      

    self.addModule = function () {
        self.modules.splice(0, 0, new Module());
    }

    self.deleteModule = function (module) {
        var result = confirm("Confirm to delete this module?");
        if (result == true) {
            self.modules.remove(module);
        }
        
    }

    self.save = function ()
    {
        $.post('/Admin/SaveModules',
             {
                 Modules: JSON.stringify(ko.toJS(self.modules)),
                 path: $('#path').text()
             },
              function (data) {
                  if (data == "success") {
                      alert("Module saved");
                  }
              });
    }

    self.loadModules();
    return self;
};


vm = new viewModel();
ko.cleanNode(document.getElementById('module'));
ko.applyBindings(vm, document.getElementById("module"));