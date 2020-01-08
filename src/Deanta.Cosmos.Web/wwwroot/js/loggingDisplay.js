﻿var LoggingDisplay = (function($) {
    'use strict';

    var logApiUrl;
    var logs = null;
    var traceIdentLocal;

    var $showLogsLink = $('#show-logs');
    var $logModal = $('#log-modal');
    var $logModalBody = $logModal.find('.modal-body');
    var $logDisplaySelect = $logModal.find('#displaySelect');

    function getDisplayType() {
        return $logDisplaySelect.find('input[name=displaySelect]:checked').val();
    }

    function updateLogsCountDisplay() {
        var allCount = logs.requestLogs.length;
        $('.modal-title span').text(allCount);
        $('#all-select span').text(allCount);
        var sqlCount = 0;
        for (var i = 0; i < logs.requestLogs.length; i++) {
            if (logs.requestLogs[i].isDb) {
                sqlCount++;
            }
        }
        $('#sql-select span').text(sqlCount);
    }

    function showModal() {
        updateLogsCountDisplay();
        $logModal.modal();
    }

    function setContextualColors(eventType) {
        switch (eventType) {
            case 'Information':
                return 'text-info';
            case 'Warning':
                return 'text-warning';
            case 'Error':
                return 'text-danger';
            case 'Critical':
                return 'text-danger bold';
            default:
                return '';
        }
    }

    function fillModalBody() {
        var displayType = getDisplayType();
        var body = ''; //'<div class="card-group" id="log-accordian" role="tablist" aria-multiselectable="true">';
        for (var i = 0; i < logs.requestLogs.length; i++) {
            if (displayType !== 'sql' || logs.requestLogs[i].isDb)
            body +=     
'<div class="card">'+
   '<div class ="card-heading" role="tab" id="heading'+i+'">'+
      '<h4 class ="card-title text-overflow-dots">'+
        '<a role="button" data-toggle="collapse" href="#collapse' + i + '" aria-expanded="false" aria-controls="collapse' + i + '">' +
          '<span class="' + setContextualColors(logs.requestLogs[i].logLevel) + '">' + logs.requestLogs[i].logLevel + ':&nbsp;</span>'+
            logs.requestLogs[i].eventString + 
        '</a>'+
      '</h4>'+
    '</div>'+
    '<div id="collapse'+i+'" class ="collapse">'+
            '<div class ="card-body white-space-pre">' + logs.requestLogs[i].eventString+''+
       '</div>'+
  '</div>'+
'</div>';
        }
        //body += '</div>';
        $logModalBody.html(body);
    }

    function getLogs(traceIdentifier) {
        $.ajax({
            url: logApiUrl,
            data: { traceIdentifier: traceIdentifier }
        })
        .done(function (data) {
            logs = data;
            fillModalBody();
            showModal();
        })
        .fail(function () {
            alert("error");
        });
    }

    function startModal() {
        getLogs(traceIdentLocal);
    }

    function setupTrace(traceIdentifier, numLogs) {
        //now set up the log menu link
        traceIdentLocal = traceIdentifier;
        $showLogsLink.find('.badge').text(numLogs + '+');
    }

    //public parts
    return {
        initialise: function(exLogApiUrl, traceIdentifier, numLogs) {
            logApiUrl = exLogApiUrl;

            setupTrace(traceIdentifier, numLogs);

             the events
            $showLogsLink.unbind('click')
                .bind('click',
                    function() {
                        startModal();
                    });
            $showLogsLink.removeClass('hidden');
            $logDisplaySelect.unbind('change')
                .bind('change',
                    function() {
                        fillModalBody();
                    });
        },

        newTrace: function(traceIdentifier, numLogs) {
            setupTrace(traceIdentifier, numLogs);
        }
    };
}(window.jQuery));