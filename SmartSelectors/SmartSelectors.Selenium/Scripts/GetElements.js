function getElementsByXPath(xpath, parent) {
    var results = [];
    var query = document.evaluate(xpath, parent || document,
        null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
    for (let i = 0, length = query.snapshotLength; i < length; ++i) {
        results.push(query.snapshotItem(i));
    }
    return results;
}

var all_elements = getElementsByXPath("//body//*[not(child::*)]");
var elements = [];


all_elements.filter(x => x.scrollWidth !== null && x.scrollHeight !== null)
    .filter(x => x.scrollWidth > 9 && x.scrollHeight > 9 &&
    x.scrollWidth < 75 && x.scrollHeight < 75 &&
    x.scrollWidth / x.scrollHeight > 0.75 &&
    x.scrollWidth / x.scrollHeight < 1.75).forEach(function (element) {
    elements = elements.filter(x => x !== element);
    elements.push(element);
    });

all_elements.filter(x => x.offsetWidth !== null && x.offsetHeight !== null)
    .filter(x => x.offsetWidth > 9 && x.offsetHeight > 9 &&
    x.offsetWidth < 75 && x.offsetHeight < 75 &&
    x.offsetWidth / x.offsetHeight > 0.75 &&
    x.offsetWidth / x.offsetHeight < 1.75).forEach(function (element) {
    elements = elements.filter(x => x !== element);
    elements.push(element);
    });

all_elements.filter(x => x.tagName === "path" && x.ownerSVGElement !== null)
    .filter(x => x.ownerSVGElement.scrollWidth !== null && x.ownerSVGElement.scrollHeight !== null)
    .filter(x => x.ownerSVGElement.scrollWidth > 9 && x.ownerSVGElement.scrollHeight > 9 &&
    x.ownerSVGElement.scrollWidth < 75 && x.ownerSVGElement.scrollHeight < 75 &&
    x.ownerSVGElement.scrollWidth / x.ownerSVGElement.scrollHeight > 0.75 &&
    x.ownerSVGElement.scrollWidth / x.ownerSVGElement.scrollHeight < 1.75).forEach(function (element) {
    elements = elements.filter(x => x !== element);
    elements.push(element);
});

var svgElements = document.querySelectorAll("svg");
svgElements = Array.from(svgElements);
svgElements.filter(x => x.scrollWidth !== null && x.scrollHeight !== null)
    .filter(x => x.scrollWidth > 9 && x.scrollHeight > 9 &&
    x.scrollWidth < 75 && x.scrollHeight < 75 &&
    x.scrollWidth / x.scrollHeight > 0.75 &&
    x.scrollWidth / x.scrollHeight < 1.75).forEach(function(element) {
    elements = elements.filter(x => x !== element);
    elements.push(element);
});

// ReSharper disable once ReturnFromGlobalScopetWithValue
return elements;