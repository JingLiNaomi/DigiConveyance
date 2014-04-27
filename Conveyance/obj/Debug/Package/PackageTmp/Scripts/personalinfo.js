//Model
function City(data) {
    var self = this;
    data = data || {};
    self.id = data.CityID;
    self.name = data.CityName;
}

function viewModel() {
    var self = this;
    self.FirstName = ko.observable();
    self.LastName = ko.observable();
    self.Email = ko.observable();
    self.Tel = ko.observable();
    self.City = ko.observable();
    self.Postcode = ko.observable();
    self.AddressLine1 = ko.observable();
    self.AddressLine2 = ko.observable();
    self.Webpage = ko.observable();
    self.errmsg = ko.observable('');
    //    self.cities = [{ id: 1, name: 'Red' }, { id: 2, name: 'blue' }, { id: 3, name: 'black' }];
    self.cities=ko.observableArray();
    self.loadCities = function ()
    {
        $.ajax({
            url: '/Account/LoadCities',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST'
        })
           .done(function (data) {
               var mappedItems = $.map(data, function (item) { return new City(item); });
               self.cities(mappedItems);
           })
        ;
    }

    self.loadUserInfo = function () {
        $.ajax({
            url: '/Account/LoadUserInfo',
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST'
        })
            .done(function (data) {
                self.FirstName(data.FirstName);
                self.LastName(data.LastName || "");
                self.Email(data.Email || "");
                self.Tel(data.Tel || "");
                self.City(data.City);
                self.Postcode(data.Postcode || "");
                self.AddressLine1(data.AddressLine1 || "");
                self.AddressLine2(data.AddressLine2 || "");
                self.Webpage(data.Webpage || "");
            })
        ;
    }

  

    self.save = function () {
        $.post('/Account/SaveUserInfo',
             {
                 jsonUserInfo: JSON.stringify(ko.toJS(
                     { FirstName:self.FirstName,
                     LastName:self.LastName,
                     Email:self.Email,
                     Tel:self.Tel,
                     City:self.City,
                     Postcode: self.Postcode,
                     AddressLine1:self.AddressLine1,
                     AddressLine2: self.AddressLine2,
                     Webpage:self.Webpage}))
             },
              function (data) {
                  if (data == "success") {
                      alert("Saved successfully");
                  }
                  else
                  {
                      alert("error, please try again later");
                  }
              });
    }

    self.loadCities();
    self.loadUserInfo();
    return self;
};


vm = new viewModel();
ko.cleanNode(document.getElementById('personalinfo'));
ko.applyBindings(vm, document.getElementById("personalinfo"));