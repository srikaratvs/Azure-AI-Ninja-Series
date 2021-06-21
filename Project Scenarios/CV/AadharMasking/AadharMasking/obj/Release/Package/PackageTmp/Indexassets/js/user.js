
$('#OutSection').hide();

//function getOrientation(file, callback) {
//    var reader = new FileReader();
//    reader.onload = function (e) {

//        var view = new DataView(e.target.result);
//        if (view.getUint16(0, false) != 0xFFD8) {
//            return callback(-2);
//        }
//        var length = view.byteLength, offset = 2;
//        while (offset < length) {
//            if (view.getUint16(offset + 2, false) <= 8) return callback(-1);
//            var marker = view.getUint16(offset, false);
//            offset += 2;
//            if (marker == 0xFFE1) {
//                if (view.getUint32(offset += 2, false) != 0x45786966) {
//                    return callback(-1);
//                }

//                var little = view.getUint16(offset += 6, false) == 0x4949;
//                offset += view.getUint32(offset + 4, little);
//                var tags = view.getUint16(offset, little);
//                offset += 2;
//                for (var i = 0; i < tags; i++) {
//                    if (view.getUint16(offset + (i * 12), little) == 0x0112) {
//                        return callback(view.getUint16(offset + (i * 12) + 8, little));
//                    }
//                }
//            }
//            else if ((marker & 0xFF00) != 0xFF00) {
//                break;
//            }
//            else {
//                offset += view.getUint16(offset, false);
//            }
//        }
//        return callback(-1);
//    };
//    reader.readAsArrayBuffer(file);
//}


//function getDataUrl(file, callback2) {
//    var callback = function (srcOrientation) {
//        var reader2 = new FileReader();
//        reader2.onload = function (e) {
//            var srcBase64 = e.target.result;
//            var img = new Image();

//            img.onload = function () {
//                var width = img.width,
//                    height = img.height,
//                    canvas = document.createElement('canvas'),
//                    ctx = canvas.getContext("2d");

//                // set proper canvas dimensions before transform & export
//                if (4 < srcOrientation && srcOrientation < 9) {
//                    canvas.width = height;
//                    canvas.height = width;
//                } else {
//                    canvas.width = width;
//                    canvas.height = height;
//                }

//                // transform context before drawing image
//                switch (srcOrientation) {
//                    case 2: ctx.transform(-1, 0, 0, 1, width, 0); break;
//                    case 3: ctx.transform(-1, 0, 0, -1, width, height); break;
//                    case 4: ctx.transform(1, 0, 0, -1, 0, height); break;
//                    case 5: ctx.transform(0, 1, 1, 0, 0, 0); break;
//                    case 6: ctx.transform(0, 1, -1, 0, height, 0); break;
//                    case 7: ctx.transform(0, -1, -1, 0, height, width); break;
//                    case 8: ctx.transform(0, -1, 1, 0, 0, width); break;
//                    default: break;
//                }

//                // draw image
//                //ctx.drawImage(video, 0, 0, width, height);
//                ctx.drawImage(img, 0, 0);

//                // export base64
//                //callback2(canvas.toDataURL());
//                callback2(canvas.toDataURL("image/jpg"));
//            };

//            img.src = srcBase64;
//        }

//        reader2.readAsDataURL(file);
//    }

//    var reader = new FileReader();
//    reader.onload = function (e) {

//        var view = new DataView(e.target.result);
//        if (view.getUint16(0, false) != 0xFFD8) return callback(-2);
//        var length = view.byteLength, offset = 2;
//        while (offset < length) {
//            var marker = view.getUint16(offset, false);
//            offset += 2;
//            if (marker == 0xFFE1) {
//                if (view.getUint32(offset += 2, false) != 0x45786966) return callback(-1);
//                var little = view.getUint16(offset += 6, false) == 0x4949;
//                offset += view.getUint32(offset + 4, little);
//                var tags = view.getUint16(offset, little);
//                offset += 2;
//                for (var i = 0; i < tags; i++)
//                    if (view.getUint16(offset + (i * 12), little) == 0x0112)
//                        return callback(view.getUint16(offset + (i * 12) + 8, little));
//            }
//            else if ((marker & 0xFF00) != 0xFF00) break;
//            else offset += view.getUint16(offset, false);
//        }
//        return callback(-1);
//    };
//    reader.readAsArrayBuffer(file);
//}
//var FileName = null, Angle=null;

$(function () {
    $(":file").change(function () {
        $('#OutSection').hide();
        if (this.files && this.files[0]) {
            //FileName = this.files[0];
            var reader = new FileReader();
            reader.onload = imageIsLoaded;
            reader.readAsDataURL(this.files[0]);
        }
    });
});



function imageIsLoaded(e) {
    $('#preview_img').attr('src', e.target.result);
    //getOrientation(FileName, function (angle) {
    //    Angle = angle;
        //alert(angle);
        //var img = document.getElementById('preview_img');
        //img.style.transform = 'rotate(180deg)';
        //if (angle >= 1 && angle <= 8) {
        //    switch (angle) {
        //        case 2:
        //            //img.style.transform = 'rotate(180deg)';
        //            //image.RotateFlip(RotateFlipType.RotateNoneFlipX);
        //            break;
        //        case 3:
        //            img.style.transform = 'rotate(180deg)';
        //            break;
        //        case 4:
        //            //image.RotateFlip(RotateFlipType.Rotate180FlipX);
        //            break;
        //        case 5:
        //            //image.RotateFlip(RotateFlipType.Rotate90FlipX);
        //            break;
        //        case 6:
        //            img.style.transform = 'rotate(270deg)';
        //            break;
        //        case 7:
        //            //image.RotateFlip(RotateFlipType.Rotate270FlipX);
        //            break;
        //        case 8:
        //            img.style.transform = 'rotate(90deg)';
        //            break;
        //    }
        //}

    //});    
}

//var input = document.getElementById('ImageFile');

//input.onchange = function (e) {
//    $('#OutSection').hide();
//    getOrientation(input.files[0], function (angle) {
//        //alert(angle);
//        $('#preview_img').attr('src', imgBase64);
//        //var img = document.getElementById('myimage');
//        //img.style.transform = 'rotate(180deg)';
//        //imageIsLoaded(imgBase64);
//    });
//}

//input.onchange = function (e) {
//    $('#OutSection').hide();
//    getDataUrl(input.files[0], function (imgBase64) {
//        $('#preview_img').attr('src', imgBase64);
//        //imageIsLoaded(imgBase64);
//    });
//}




//$(function () {
//    $(":file").change(function () {
//        $('#OutSection').hide();
//        if (this.files && this.files[0]) {
//            getDataUrl(this.files[0], function (imgBase64) {                
//                $('#preview_img').attr('src', imgBase64);
//            });
//        };
//    });
//});

    $('#process_btn').on("click", function () {
        $('#OutSection').hide();
        var Url = "/Home/TataAIGVision/";
        var preview_img_src = $('#preview_img').attr('src');
        //console.log(preview_img_src);

        if (preview_img_src == "/TATAAIGassets/img/preview.png") {
            $('#process_error_msg').html("Please upload a valid file");
            $("#process_error_msg").css("color", "red");
        }
        else {
            var img_64_array = preview_img_src.split(',');

            var get_loading_text = $(this).attr('data-loading-text');

            $('#process_btn').html(get_loading_text);
            $('#process_error_msg').html("");
            $("#process_btn").attr("disabled", true);

            var UserData = JSON.stringify({
                ImageData: img_64_array[1]
            });
            console.log(UserData);

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf - 8",
                datatype: "json",
                data: UserData,
                url: Url,
                success: function (response) {

                    console.log(response);
                    try {
                        if (response.StatusCode == "200") {
                            $('#OutSection').show();
                            $('#process_btn').html("Process");
                            $("#process_btn").attr("disabled", false);

                            $('#out_preview_img').attr('src', response.Message);

                        } else if (response.StatusCode == "400") {
                            $('#process_error_msg').html(response.Message);
                            $("#process_error_msg").css("color", "red");
                            $('#process_btn').html("Process");
                            $("#process_btn").attr("disabled", false);
                        } else {
                            $('#process_error_msg').html("");
                            $("#process_error_msg").css("color", "red");
                            $('#process_btn').html("Process");
                            $("#process_btn").attr("disabled", false);
                        }
                    } catch (err) {
                        console.log(err);
                        $('#process_error_msg').html("Please try again");
                        $("#process_error_msg").css("color", "red");
                        $('#process_btn').html("Process");
                        $("#process_btn").attr("disabled", false);
                    }
                },
                error: function (response) {
                    console.log(response);
                    $('#process_error_msg').html("Please try again");
                    $("#process_error_msg").css("color", "red");
                    $('#process_btn').html("Process");
                    $("#process_btn").attr("disabled", false);
                }
            });
        }
    });



    $('#logout_btn').on("click", function (event) {
        event.preventDefault();
        var Url = "/Home/LogoutCheck/";
        var ResData = "";
        $.ajax({
            type: "POST",
            contentType: "application/json",
            datatype: "json",
            data: ResData,
            url: Url,
            success: function (response) {
                console.log(response);
                try {
                    if (response.StatusCode == "200") {
                        location.href = '/Home/Index';
                    }
                    else {
                        alert("Something went wrong,Please try again");
                    }
                } catch (err) {
                    alert("Something went wrong,Please try again");
                }
            },
            error: function (response) {
                alert("Something went wrong,Please try again");
            }
        });

    });


    $(document).ready(function () {
        $('#cam_sec').hide();
        var scaleFactor = 1;
        var video = document.querySelector('video');

        $('#on_off_btn').on("click", function () {
            if ($(this).prop("checked") == true) {
                webcam();
                $('#preview_img').attr('src', '/TATAAIGassets/img/preview.png');
                $('#cam_sec').show();
                $('#upload_sec').hide();
            }
            else if ($(this).prop("checked") == false) {
                $('#preview_img').attr('src', '/TATAAIGassets/img/preview.png');
                $('#ImageFile').val("");
                $('#cam_sec').hide();
                $('#upload_sec').show();

            }
            $('#OutSection').hide();
            $('#process_error_msg').html("");

        });


        $('#capture_btn').on("click", function () {
            $('#OutSection').hide();
            $('#process_error_msg').html("");
            var canvas = capture(video, scaleFactor);
            var capture_image = canvas.toDataURL("image/png");
            $('#preview_img').attr('src', capture_image);
        });

    });

    //Capture Image Function
    function capture(video, scaleFactor) {
        if (scaleFactor == null) {
            scaleFactor = 1;
        }
        var w = video.videoWidth * scaleFactor;
        var h = video.videoHeight * scaleFactor;
        var canvas = document.createElement('canvas');
        canvas.width = w;
        canvas.height = h;
        var ctx = canvas.getContext('2d');
        ctx.drawImage(video, 0, 0, w, h);
        return canvas;
    }


    function webcam() {
        navigator.getUserMedia = (navigator.getUserMedia ||
            navigator.webkitGetUserMedia ||
            navigator.mozGetUserMedia ||
            navigator.msGetUserMedia
        );
        if (navigator.getUserMedia) {
            navigator.getUserMedia(
                // constraints
                {
                    video: true,
                    audio: false
                },
                // successCallback
                function (localMediaStream) {
                    video = document.querySelector('video');
                    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
                        // Not adding `{ audio: true }` since we only want video now
                        navigator.mediaDevices.getUserMedia({
                            video: true
                        }).then(function (stream) {
                            video.srcObject = stream;
                            video.play();
                        });
                    }
                },
                // errorCallback
                function (err) {
                    console.log(err);
                }
            );
        }
        else {
            console.log("Camera Not Available");
        }
    }
