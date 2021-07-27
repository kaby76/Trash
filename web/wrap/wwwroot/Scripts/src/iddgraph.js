var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "./contextgraph", "idd"], function (require, exports, ContextGraph) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var InteractiveDataDisplay = require('idd');
    var IDDGraph = (function (_super) {
        __extends(IDDGraph, _super);
        function IDDGraph(chartID, graph) {
            var _this = _super.call(this) || this;
            _this.grid = false;
            var plotContainerID = chartID + '-container';
            var plotID = chartID + '-plot';
            var container = document.getElementById(chartID);
            container.setAttribute('data-idd-plot', 'plot');
            var containerDiv = document.createElement('div');
            containerDiv.setAttribute('id', plotContainerID);
            containerDiv.setAttribute('data-idd-plot', plotID);
            container.appendChild(containerDiv);
            IDDGraph.msaglPlot.prototype = new InteractiveDataDisplay.CanvasPlot;
            _this.graph = graph === undefined ? null : graph;
            InteractiveDataDisplay.register(plotID, function (jqDiv, master) { return new IDDGraph.msaglPlot(jqDiv, master); });
            var chart = InteractiveDataDisplay.asPlot(chartID);
            var gestureSource = InteractiveDataDisplay.Gestures.getGesturesStream($("#" + chartID));
            chart.navigation.gestureSource = gestureSource;
            container.ondblclick = function (ev) {
                chart.fitToView();
            };
            _this.gplot = chart.get(plotContainerID);
            return _this;
        }
        IDDGraph.prototype.drawGraph = function () {
            this.gplot.setGraph(this);
            this.gplot.invalidateLocalBounds();
            this.gplot.requestNextFrameOrUpdate();
        };
        IDDGraph.prototype.drawGraphFromPlot = function (context) {
            if (this.grid)
                this.drawGrid(context);
            this.drawGraphInternal(context, this.graph);
        };
        IDDGraph.msaglPlot = function (jqDiv, master) {
            this.base = InteractiveDataDisplay.CanvasPlot;
            this.base(jqDiv, master);
            this.aspectRatio = 1.0;
            var _graph;
            this.renderCore = function (plotRect, screenSize) {
                if (_graph && plotRect.width > 0 && plotRect.height > 0) {
                    var bbox = _graph.graph.boundingBox;
                    var context = this.getContext(true);
                    context.save();
                    var t = this.coordinateTransform;
                    var offset = t.getOffset();
                    var scale = t.getScale();
                    context.translate(offset.x, offset.y);
                    context.scale(scale.x, scale.y);
                    context.translate(-bbox.x, -bbox.height - bbox.y);
                    _graph.drawGraphFromPlot(context);
                    context.restore();
                }
            };
            this.setGraph = function (g) { _graph = g; };
            this.computeLocalBounds = function (step, computedBounds) {
                return _graph ? { x: 0, y: 0, width: _graph.graph.boundingBox.width, height: _graph.graph.boundingBox.height } : { x: 0, y: 0, width: 1, height: 1 };
            };
        };
        return IDDGraph;
    }(ContextGraph));
});
//# sourceMappingURL=iddgraph.js.map