function getElementsByXPath(xpath, parent) {
    let results = [];
    let query = document.evaluate(xpath, parent || document,
        null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
    for (let i = 0, length = query.snapshotLength; i < length; ++i) {
        results.push(query.snapshotItem(i));
    }
    return results;
}

var elements = [];

getElementsByXPath("//body//*[not(child::*)]").filter(x => x.offsetWidth > 10 && x.offsetHeight > 10 &&
    x.offsetWidth < 75 && x.offsetHeight < 75 &&
    x.offsetWidth / x.offsetHeight > 0.75 &&
    x.offsetWidth / x.offsetHeight < 1.75).forEach(function (element) {
    elements = elements.filter(x => x !== element);
    elements = elements.filter(x => x !== element.firstChild);
    elements.push(element);
});
// ReSharper disable once ReturnFromGlobalScopetWithValue
return elements;