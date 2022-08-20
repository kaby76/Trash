define(["require", "exports", "./Scripts/src/ggraph", "./Scripts/src/iddsvggraph"], function (require, exports, G, IDDSVGGraph) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var graphView = document.getElementById("graphView");
    var graphControl = new IDDSVGGraph(graphView);
    var graph = null;
    var nodeCountControl = document.getElementById("nodeCount");
    var edgeCountControl = document.getElementById("edgeCount");
    var startButton = document.getElementById("startButton");
    var stopButton = document.getElementById("stopButton");
    var working = document.getElementById("working");
    var elapsed = document.getElementById("elapsed");
    var edgeRoutingSelect = document.getElementById("edgeRoutingSelect");
    function setGUIToRunning() {
        working.style.display = "inline";
        startButton.disabled = true;
        stopButton.disabled = false;
        elapsed.textContent = "";
    }
    function setGUIToNotRunning() {
        working.style.display = "none";
        startButton.disabled = false;
        stopButton.disabled = true;
    }
    function run(nodeCount, edgeCount) {
        graph = new G.GGraph();
        graphControl.setGraph(graph);
        graph.settings.aspectRatio = graphView.offsetWidth / graphView.offsetHeight;
        graph.settings.routing = edgeRoutingSelect.value;
        for (var i = 1; i <= nodeCount; i++)
            graph.addNode(new G.GNode({ id: "node" + i, label: "Node " + i }));
        var randomness = new Uint32Array(edgeCount * 2);
        if (typeof (crypto) != "undefined")
            crypto.getRandomValues(randomness);
        else
            window.msCrypto.getRandomValues(randomness);
        for (var i = 1; i <= edgeCount; i++)
            graph.addEdge(new G.GEdge({
                id: "edge" + i, source: "node" + (randomness[(i - 1) * 2] % nodeCount + 1),
                target: "node" + (randomness[(i - 1) * 2 + 1] % nodeCount + 1)
            }));
        var startTime = new Date();
        graph.createNodeBoundariesForSVGInContainer(graphView);
        graph.layoutCallbacks.add(function () {
            graphControl.drawGraph();
            setGUIToNotRunning();
            var endTime = new Date();
            var diff = endTime.getTime() - startTime.getTime();
            elapsed.textContent = diff + " msecs";
        });
        graph.beginLayoutGraph();
    }
    function startButtonClicked() {
        var nodeCount = parseInt(nodeCountControl.value);
        var edgeCount = parseInt(edgeCountControl.value);
        setGUIToRunning();
        run(nodeCount, edgeCount);
    }
    function stopButtonClicked() {
        graph.stopLayoutGraph();
        setGUIToNotRunning();
    }
    function showPrompt(message) {
        return prompt(message, 'Type anything here');
    }
});
//# sourceMappingURL=app.js.map