// mirror the LINQ select projection
Array.prototype.select = function (a) {
    
    if (!$.isFunction(a)) {
        return this;
    }

    var target = [];

    for (var i = 0; i < this.length; i++) {
        target.push(a(this[i]));
    }

    return target;
}