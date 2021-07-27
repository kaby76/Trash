define(["require", "exports", "../../Scripts/src/ggraph", "../../Scripts/src/iddsvggraph"], function (require, exports, G, IDDSVGGraph) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var graphView = document.getElementById("graphView");
    var graphControl = new IDDSVGGraph(graphView);
    var graph = null;
    var layeredLayoutCheckBox = document.getElementById("layeredLayoutCheckBox");
    var horizontalLayoutCheckBox = document.getElementById("horizontalLayoutCheckBox");
    var aspectRatioTextBox = document.getElementById("aspectRatioTextBox");
    var edgeRoutingSelect = document.getElementById("edgeRoutingSelect");
    var workingIndicator = document.getElementById("workingIndicator");
    var jsonGraph = "";
    function showWorking(show) {
        if (show) {
            workingIndicator.style.display = "inherit";
            graphView.style.display = "none";
        }
        else {
            workingIndicator.style.display = "none";
            graphView.style.display = "";
        }
    }
    function stop() {
        if (graph != null && graph.working)
            graph.stopLayoutGraph();
        showWorking(false);
    }
    function copySettingsToGraph() {
        graph.settings.layout = layeredLayoutCheckBox.checked ? G.GSettings.sugiyamaLayout : G.GSettings.mdsLayout;
        graph.settings.transformation = horizontalLayoutCheckBox.checked ? G.GPlaneTransformation.ninetyDegreesTransformation : G.GPlaneTransformation.defaultTransformation;
        if (aspectRatioTextBox.value != null && aspectRatioTextBox.value != "")
            graph.settings.aspectRatio = parseFloat(aspectRatioTextBox.value);
        graph.settings.routing = edgeRoutingSelect.value;
    }
    function layoutClicked() {
        stop();
        copySettingsToGraph();
        graph.createNodeBoundariesForSVGInContainer(graphView);
        showWorking(true);
        graph.beginLayoutGraph();
    }
    function routeClicked() {
        stop();
        copySettingsToGraph();
        showWorking(true);
        graph.beginEdgeRouting();
    }
    function stopClicked() {
        stop();
    }
    function loadGraph(json) {
        jsonGraph = json;
        graph = G.GGraph.ofJSON(jsonGraph);
        graphControl.setGraph(graph);
        graphControl.allowEditing = false;
        graph.workStoppedCallbacks.add(function () {
            showWorking(false);
            graphControl.drawGraph();
        });
        layoutClicked();
    }
    function loadGraph1() {
        require(["text!Samples/Options/samplegraph1.json"], function (sample) {
            loadGraph(sample);
        });
    }
    function loadGraph2() {
        require(["text!Samples/Options/samplegraph2.json"], function (sample) {
            loadGraph(sample);
        });
    }
    document.getElementById("layoutButton").onclick = layoutClicked;
    document.getElementById("routeButton").onclick = routeClicked;
    document.getElementById("stopButton").onclick = stopClicked;
    document.getElementById("loadGraph1Button").onclick = loadGraph1;
    document.getElementById("loadGraph2Button").onclick = loadGraph2;
    loadGraph1();
});
//# sourceMappingURL=app.js.map