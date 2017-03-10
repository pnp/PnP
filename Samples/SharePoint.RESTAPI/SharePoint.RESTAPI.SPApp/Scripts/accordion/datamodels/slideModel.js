AccordionContent.SlideModel = function () {
    var AccordionContentSlide = function () {
        var SlideItem = function (slideWhat, slideDescription, slideWho) {
            this.slideWhat = slideWhat;
            this.slideDescription = slideDescription;
            this.slideWho = slideWho;
        };

        return {
            SlideItem: SlideItem
        }
    }();

    return {
        AccordionContentSlide: AccordionContentSlide
    }
}();