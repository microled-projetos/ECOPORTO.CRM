var links = $('.navbar ul li a');

$.each(links, function (key, va) {
    if (va.href == document.URL) {
        $(this).addClass('active');
    }
});

$(".moeda").mask('#.##0,00', {
    reverse: true,
    allowNegative: false,
    thousands: '.',
    decimal: ',',
    affixesStay: false,
    clearIfNotMatch: true
});

$(".moeda-inteiro").mask('#.##0,00', {
    reverse: true,
    allowNegative: false,
    thousands: '.',
    decimal: ',',
    affixesStay: false,
    clearIfNotMatch: false
});

$(".data").mask('00/00/0000');
$('.telefone').mask('(00) 0000-0000');
$('.celular').mask('(00) 00000-0000');
$('.cep').mask('00000-000');
$('.cpf').mask('000.000.000-00');
$('.cnpj').mask('00.000.000/0000-00');
$('.IP').mask('0ZZ.0ZZ.0ZZ.0ZZ', { translation: { 'Z': { pattern: /[0-9]/, optional: true } } });

$(".inteiro").on("keypress keyup blur", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});


$(".campo-decimal").on("keypress keyup blur", function (event) {
    
    console.log('tecla: ' + event.which);
    if (((event.which < 48 && event.which !== 44) || (event.which > 57 && event.which !== 188))) {
        event.preventDefault();
    }
});

var isNumero = function (numero) {
    return !isNaN(numero - parseFloat(numero));
}

var isInteiro = function (numero) {
    return /^\d+$/.test(numero);
}

var isMoeda = function (numero) {

    var valor = numero
        .replace(/\./g, '')
        .replace(',', '.');

    return !isNaN(valor - parseFloat(valor));
}

var formataMoedaCalculo = function (numero) {

    return numero
        .replace(/\./g, '')
        .replace(',', '.');
}

var formataMoedaPtBr = function (numero) {

    var numero = numero.toFixed(2).split('.');
    numero[0] = numero[0].split(/(?=(?:...)*$)/).join('.');
    return numero.join(',');
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

var pesquisar = function () {
    var valor = $("#btnPesquisaSite").getSelectedItemData();
    window.location.href = urlBase + 'Busca/?chave=' + valor.Chave + '&termo=' + '&menu=' + valor.Menu;
    console.log(valor);
}

var options = {
    url: function (phrase) {
        return urlBase + "Home/Buscar/?criterio=" + phrase;
    },
    getValue: "Termo",
    listLocation: "resultados",
    requestDelay: 500,
    template: {
        type: "description",
        fields: {
            description: "Menu"
        }
    },
    list: {
        onClickEvent: function () {
            pesquisar();
        },
        onKeyEnterEvent: function () {
            pesquisar();
        },
        onShowListEvent: function () {

            var termo = $('#btnPesquisaSite').val();

            $('#li-comeca-com').remove();

            var liComecaCom = document.createElement('li');
            liComecaCom.classList.add('eac-item');
            liComecaCom.setAttribute('id', 'li-comeca-com');

            var divComecaCom = document.createElement('div');
            divComecaCom.classList.add('eac-item');

            var lnkComecaCom = document.createElement('a');
            lnkComecaCom.innerHTML = '<br/><b>Começa com: </b>' + termo;
            lnkComecaCom.href = urlBase + 'Busca/?chave=&termo=' + termo;

            divComecaCom.appendChild(lnkComecaCom);
            liComecaCom.appendChild(divComecaCom);

            $('#eac-container-btnPesquisaSite > ul')
                .append(liComecaCom);
        }
    }
};

$("#btnPesquisaSite").easyAutocomplete(options);

$("input[type=text]").keyup(function () {

    var start = $(this)[0].selectionStart;
    var end = $(this)[0].selectionEnd;

    $(this).val($(this).val().toUpperCase());
    $(this).selectRange(start, end);
});

$.fn.selectRange = function (start, end) {
    $(this).each(function () {
        var el = $(this)[0];

        if (el) {
            el.focus();

            if (el.setSelectionRange) {
                el.setSelectionRange(start, end);

            } else if (el.createTextRange) {
                var range = el.createTextRange();
                range.collapse(true);
                range.moveEnd('character', end);
                range.moveStart('character', start);
                range.select();

            } else if (el.selectionStart) {
                el.selectionStart = start;
                el.selectionEnd = end;
            }
        }
    });
};

$('.btn').on('click', function () {

    var $this = $(this);
    var dataAnimation = $(this).data('animation-name');

    if (dataAnimation) {
        var loadingText = '<i class="fa fa-spinner fa-spin"></i> ' + $(this).html();
        if ($(this).html() !== loadingText) {
            $this.data('original-text', $(this).html());
            $this.html(loadingText);
        }
        setTimeout(function () {
            $this.html($this.data('original-text'));
        }, 2000);
    }
});

$('select').keyup(function (e) {

    if (e.keyCode === 46) {

        $(this).prop('selectedIndex', -1);
    }
});

$(document).on('keyup keypress', 'form input', function (e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

var Strings = {};
Strings.orEmpty = function (entity) {
    return entity || "";
};

function replaceAll(str, find, replace) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function jsonCopy(src) {
    return JSON.parse(JSON.stringify(src));
}

function toBoolean(string) {
    switch (string.toLowerCase().trim()) {
        case "true": case "yes": case "1": return true;
        case "false": case "no": case "0": case null: return false;
        default: return Boolean(string);
    }
}

$('.dropdown-menu a.dropdown-toggle').on('click', function(e) {
  if (!$(this).next().hasClass('show')) {
    $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
  }
  var $subMenu = $(this).next(".dropdown-menu");
  $subMenu.toggleClass('show');


  $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function(e) {
    $('.dropdown-submenu .show').removeClass("show");
  });


  return false;
});

String.prototype.capitalize = function () {
    return this.charAt(0).toUpperCase() + this.slice(1)
}