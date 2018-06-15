var urlContact = "/contact";
var url = window.location.pathname;
var profileId = url.substring(url.lastIndexOf('/') + 1);

$.ajax({
    url: urlContact + '/InitializePageData',
    async: false,
    dataType: 'json',
    success: function (json) {
      
    }
});

$(function () {

var Profile = function (profile) {
    var self = this;
    self.ProfileId = ko.observable(profile ? profile.ProfileId : 0).extend({ required: true });
    self.FirstName = ko.observable(profile ? profile.FirstName : '').extend({ required: true,pattern:"[a-zA-Z ]{3,}", maxLength: 50,minLength:3});
    self.LastName = ko.observable(profile ? profile.LastName : '').extend({ required: true,pattern:"[a-zA-Z ]{3,}", maxLength: 50,minLength:3});
    self.Email = ko.observable(profile ? profile.Email : '').extend({ required: true, maxLength: 50, email: true });
    self.status = ko.observable(profile ? profile.status : 0);
        };


var ProfileCollection = function () {
    var self = this;

    //if ProfileId is 0, It means Create new Profile
    if (profileId == 0) {
        self.profile = ko.observable(new Profile());
        self.profile().status = document.getElementById('sel').value;
        
    }
    else {
        $.ajax({
            url: urlContact + '/GetProfileById/' + profileId,
            async: false,
            dataType: 'json',
            success: function (json) {
                self.profile = ko.observable(new Profile(json));
               
            }
        });
    }

    self.backToProfileList = function () { window.location.href = '/contact'; };

    self.profileErrors = ko.validation.group(self.profile());
  
    self.saveProfile = function () {

        var isValid = true;

        if (self.profileErrors().length != 0) {
            self.profileErrors.showAllMessages();
            isValid = false;
        }

        if( isValid)
        {
            self.profile().ProfileId = profileId;
            self.profile().status = document.getElementById('sel').value;

            $.ajax({
                type: (self.profile().ProfileId > 0 ? 'PUT' : 'POST'), 
                cache: false,
                dataType: 'json',
                url: urlContact + (self.profile().ProfileId > 0 ? '/UpdateProfileInformation?id=' + self.profile().ProfileId : '/SaveProfileInformation'),
                data: JSON.stringify(ko.toJS(self.profile())), 
                contentType: 'application/json; charset=utf-8',
                async: false,
                success: function (data) {
                    window.location.href = '/contact';
                },
                error: function (err) {
                    var err = JSON.parse(err.responseText);
                    var errors = "";
                    for (var key in err) {
                        if (err.hasOwnProperty(key)) {
                            errors += key.replace("profile.", "") + " : " + err[key];
                        }
                    }
                    $("<div></div>").html(errors).dialog({ modal: true, title: JSON.parse(err.responseText).Message, buttons: { "Ok": function () { $(this).dialog("close"); } } }).show();
                },
                complete: function () {
                }
            });
        }
    };
};

ko.applyBindings(new ProfileCollection());
});

var clone = (function () {
    return function (obj) {
        Clone.prototype = obj;
        return new Clone()
    };
    function Clone() { }
}());