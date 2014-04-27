function viewModel() {
    var self = this;
    var userID = $("#userID").text();
    var caseID = $("#caseID").text();
    self.userName = ko.observable();
    self.counter = ko.observable(0);
    self.notifications = ko.observableArray();
    self.hub = $.connection.notificationHub;

    self.init = function () {
        if (userID != "" && caseID!="") {
            self.hub.server.initialize(userID);
            self.hub.server.getNotifications(userID, caseID);
        }
    }

    self.btnClass = ko.computed(function () {
        return this.counter() > 0 ? "btn-warning" : "btn-info";
    }, self);

    //functions called by the Hub
    self.hub.client.loadNotifications = function (data) {
        self.notifications(data);
    }

    self.hub.client.setCounter = function (num) {
        self.counter(num);
    }

    self.hub.client.newNotification = function () {
        self.hub.server.getNotifications(userID, caseID);
        self.counter(self.counter()+1);
    }
};


vmPost = new viewModel();
ko.applyBindings(vmPost);


$(function () {
    $.connection.hub.start().done(function () {
        vmPost.init();
    });
});