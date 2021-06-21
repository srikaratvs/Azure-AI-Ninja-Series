$('#login_btn').on("click", function () {

    event.preventDefault()

    $('#error_msg').html("");

    var UserName = $("#email").val();    var Password = $("#password").val();

    if (UserName == "" || Password == "") {
        $('#error_msg').html("Please fill all the fields");
        $("#error_msg").css("color", "red");
    }
    else {
        $('#error_msg').html("Please Wait..");
        $("#error_msg").css("color", "black");

        var UserData = JSON.stringify({ Login: UserName, Password: Password });

        var Url = "/Home/LoginCheck/";

        $.ajax({
            type: "POST",
            contentType: "application/json",
            datatype: "json",
            data: UserData,
            url: Url,
            success: function (response) {
                console.log(response);
                try {
                    if (response.StatusCode == "200") {
                        window.location = "/Home/Chat";
                    }
                    else if (response.StatusCode == "500") {
                        $('#error_msg').html("Invalid username or password");
                        $("#error_msg").css("color", "red");
                    }
                    else {
                        $('#error_msg').html(response);
                        $("#error_msg").css("color", "red");
                    }
                }
                catch (err) {
                    $('#error_msg').html("Please try again");
                    $("#error_msg").css("color", "red");
                }
            },
            error: function (response) {
                $('#error_msg').html("Please try again");
                $("#error_msg").css("color", "red");
            }
        });
    }
});