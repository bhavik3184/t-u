(function() {
'use strict';

var $ = jQuery;
var appendContent = true;

 

function updateContent() {

    var $panel = $('.no5');
    var $panelContent = $('.no5 > .sp-viewport > .sp-container');
    var length = $panelContent.children().length;

    if (length <= 0) {
        appendContent = true;
    } else if (length >= 10) {
        appendContent = false;
    }

    

    $panel.scrollpanel('update');
}

function init() {

   
    $('.scrollpanel').scrollpanel();

    setInterval(updateContent, 1000);
}

$(init);

}());
