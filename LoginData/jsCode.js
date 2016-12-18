javascript: ( 
    function() 
    {
        var jqUISrc = 'https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.12.1/jquery-ui.js';
        var accountFile = "https://raw.githubusercontent.com/lelehehe/SubTitle/master/LoginData/accounts.json";

        var link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = 'https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.12.1/jquery-ui.min.css';
        document.head.appendChild(link);

        var a = document.createElement("script");
        a.src = '//cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js';
        a.onload = function () {
            $.getScript( jqUISrc, function( data, textStatus, jqxhr ) {
                console.log( textStatus ); 
                $.getJSON(accountFile, function (acounts) {
                    var myMenu = $( '<ul id="myMenu" style="position: absolute;z-index: 10;"></ul>' )
                    .appendTo( "body" );
                    var output = [];

                    $.each(acounts, function(key, account)
                    {
                        output.push('<li><a href="#" tag="'+ account.pwd +'">'+ account.id +'</div></li>');
                    });

                    $('#myMenu').html(output.join(''));
                    $('#myMenu').menu({
                        select: function( event, ui ) {
                            var id = ui.item.children().text();
                            var pwd = ui.item.children().attr('tag');
                            $('input.firstFocus').val(id);
                            $('input[type="password"]').val(pwd);
                            $('button').click();
                        }
                    });
                })
            });
        };
        document.head.appendChild(a);
    })();