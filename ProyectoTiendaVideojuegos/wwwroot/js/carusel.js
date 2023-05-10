    var cont = 0;
    function loopSlider() {
        var xx = setInterval(function () {
            switch (cont) {
                case 0: {
                    $("#slider-1").fadeOut(300);
                    $("#slider-2").delay(300).fadeIn(300);
                    $("#sButton1").removeClass("bg-purple-800");
                    $("#sButton2").addClass("bg-purple-800");
                    cont = 1;
                    break;
                }
                case 1:
                    {
                        $("#slider-2").fadeOut(300);
                        $("#slider-1").delay(300).fadeIn(300);
                        $("#sButton2").removeClass("bg-purple-800");
                        $("#sButton1").addClass("bg-purple-800");
                        cont = 0;
                        break;
                    }
            }
        }, 8000);
    }
    function reinitLoop(time) {
        clearInterval(xx);
        setTimeout(loopSlider(), time);
    }
    function sliderButton1() {
        $("#slider-2").fadeOut(300);
        $("#slider-1").delay(300).fadeIn(300);
        $("#sButton2").removeClass("bg-purple-800");
        $("#sButton1").addClass("bg-purple-800");
        reinitLoop(3000);
        cont = 0
    }
    function sliderButton2() {
        $("#slider-1").fadeOut(300);
        $("#slider-2").delay(300).fadeIn(300);
        $("#sButton1").removeClass("bg-purple-800");
        $("#sButton2").addClass("bg-purple-800");
        reinitLoop(3000);
        cont = 1
    }
    $(window).ready(function () {
        $("#slider-2").hide();
        $("#sButton1").addClass("bg-purple-800");
        loopSlider();
    });

