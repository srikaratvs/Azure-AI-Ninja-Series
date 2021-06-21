$(document).ready(function () {

    $("#login_btn_click").click(function (event) {
        event.preventDefault()
        $('#error_msg').html("");
        var user_name = $('#user_name').val();
        var password = $('#password').val();
        if (user_name == "" || password == "") {
            $('#error_msg').html("Please fill all the fields");
            $("#error_msg").css("color", "red");
        }
        else {
            $('#error_msg').html("Please Wait..");
            $("#error_msg").css("color", "black");
            var get_data = $(this).attr('loading-text');
            $("#login_btn_click").html(get_data);
            $('#login_btn_click').button('loading');
            $("#login_btn_click").attr("disabled", true);
            var login_data = JSON.stringify({
                user_name: user_name,
                password: password
            });
            var Url = "/Home/LoginCheck/";
            $.ajax({
                type: "POST",
                contentType: "application/json",
                datatype: "json",
                data: login_data,
                url: Url,
                success: function (response) {
                    console.log(response);
                    try {
                        if (response.Result == "100") {
                            window.location = "/Home/Process/";
                        }
                        else if (response.Result == "200") {
                            $("#login_btn_click").html("Login");
                            $("#login_btn_click").attr("disabled", false);
                            $('#error_msg').html("Invalid username or password");
                            $("#error_msg").css("color", "red");
                        }
                        else {
                            $("#login_btn_click").html("Login");
                            $("#login_btn_click").attr("disabled", false);
                            $('#error_msg').html(response);
                            $("#error_msg").css("color", "red");
                        }

                    }
                    catch (err) {
                        $("#login_btn_click").html("Login");
                        $("#login_btn_click").attr("disabled", false);
                        $('#error_msg').html("Please try again");
                        $("#error_msg").css("color", "red");
                    }
                },
                error: function (response) {
                    $("#login_btn_click").html("Login");
                    $("#login_btn_click").attr("disabled", false);
                    $('#error_msg').html("Please try again");
                    $("#error_msg").css("color", "red");
                }
            });
        }
    });

});