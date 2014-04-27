function viewModel() {
    var self = this;
    var userID = $("#userID").text();
    var caseID = $("#caseID").text();
    self.userName = ko.observable();
    var head = "http://";
    self.hub = $.connection.notificationHub;


    self.init = function () {
        if (userID != "") {
            self.hub.server.initialize(userID);
        }
    }

    self.addInvitation = function () {
        if (!self.userName()) {
            alert("Please enter solicitor username");
            return;
        }
        self.hub.server.addInvitation({ "Message": "invication to case", "SenderID": userID, "ReceiverID": self.userName(), "URL": head.concat(location.host, "/Cases/EnrollSolicitor/", caseID), "IsInvitation": true, "CaseID": caseID });
    }

    //functions called by the Hub
    self.hub.client.acknowledge = function () {
        alert("Invitation sent");
        self.userName("");
    }
    self.hub.client.error = function (errmsg) {
        alert(errmsg);
        self.userName("");
    }
};


vmPost = new viewModel();
ko.applyBindings(vmPost);


$(function () {
    $.connection.hub.start().done(function () {
        vmPost.init();
    });
});