//Fix for number and range validation with different cultures.
//Credit to http://blog.rebuildall.net/2011/03/02/jquery_validate_and_the_comma_decimal_separator
//NOTE: As of the moment of development there is a better solution to fix validation with different
//cultures using the jquery/globalize plugin, but since it doesn't support IE8 it could not be used.

$.validator.methods.range = function (value, element, param) {
    var globalizedValue = value.replace(",", ".");
    return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
}

$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
};