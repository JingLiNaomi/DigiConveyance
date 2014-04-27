
$(document).ready(function () {
    tab = $('#tab').text();
    if (tab == "A")
    {
        $('#stageA').addClass("active");
        $('#TabA').addClass("active");
    }
    else if (tab == "B")
    {
        $('#TabB').addClass("active");
        $('#stageB').addClass("active");
    }
    else if (tab == "C")
    {
        $('#TabC').addClass("active");
        $('#stageC').addClass("active");
    }
    else if (tab == "D")
    {
        $('#TabD').addClass("active");
        $('#stageD').addClass("active");
    }
       
    else if (tab == "E")
    {
        $('#TabE').addClass("active");
        $('#stageE').addClass("active");
    }
    else if (tab == "F")
    {
        $('#TabF').addClass("active");
        $('#stageF').addClass("active");
    }
       
});