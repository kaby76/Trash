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
define(["require", "exports", "./ggraph", "./svggraph", "idd"], function (require, exports, G, SVGGraph) {
    "use strict";
    var InteractiveDataDisplay = require('idd');
    var IDDSVGGraph = (function (_super) {
        __extends(IDDSVGGraph, _super);
        function IDDSVGGraph(container, graph) {
            var _this = _super.call(this, container, graph) || this;
            _this.referenceScale = 1.0;
            _this.zoomLevelChangeCallback = function (level) { };
            _this.gestureSource = undefined;
            var that = _this;
            var chartID = container.id;
            var c = 1;
            if (chartID == "" || document.getElementById(chartID) != container)
                while (chartID == "" || document.getElementById(chartID) != null)
                    chartID = "id_" + c++;
            container.setAttribute("id", chartID);
            var plotContainerID = chartID + '-container';
            var plotID = chartID + '-plot';
            container.setAttribute('data-idd-plot', 'plot');
            var containerDiv = document.getElementById(plotContainerID);
            if (containerDiv == undefined) {
                containerDiv = document.createElement('div');
                containerDiv.setAttribute('id', plotContainerID);
                containerDiv.style.width = '100%';
                containerDiv.style.height = '100%';
                containerDiv.style.minWidth = '100px';
                containerDiv.style.minHeight = '100px';
                containerDiv.setAttribute('data-idd-plot', plotID);
                container.appendChild(containerDiv);
            }
            IDDSVGGraph.msaglPlot.prototype = new InteractiveDataDisplay.Plot;
            InteractiveDataDisplay.register(plotID, function (jqDiv, master) { return new IDDSVGGraph.msaglPlot(that, jqDiv, master); });
            _this.chart = InteractiveDataDisplay.asPlot(chartID);
            _this.chart.aspectRatio = 1;
            var gestureSource = InteractiveDataDisplay.Gestures.getGesturesStream($("#" + chartID));
            _this.chart.navigation.gestureSource = gestureSource;
            _this.gplot = _this.chart.get(plotContainerID);
            _this.chart.master.host.on('widthScaleChanged', function (event, data) {
                var newWidthScale = data['widthScale'];
                var zoomLevel = newWidthScale / that.referenceScale;
                that.zoomLevelChangeCallback(zoomLevel);
            });
            _this.containerRect = container.getBoundingClientRect();
            if (container.msagl_check_size_interval != null)
                clearInterval(container.msagl_check_size_interval);
            container.msagl_check_size_interval = setInterval(function () { return that.checkSizeChanged(); }, 300);
            return _this;
        }
        IDDSVGGraph.prototype.checkSizeChanged = function () {
            var rect = this.container.getBoundingClientRect();
            for (var i in rect)
                if (rect[i] != this.containerRect[i]) {
                    InteractiveDataDisplay.updateLayouts($(this.container));
                    break;
                }
            this.containerRect = rect;
        };
        IDDSVGGraph.prototype.getViewBox = function () {
            var vb = this.gplot.svg.getAttribute("viewBox");
            if (vb == null || vb == "")
                return null;
            var tokens = vb.split(' ');
            var x = parseFloat(tokens[0]);
            var y = parseFloat(tokens[1]);
            var width = parseFloat(tokens[2]);
            var height = parseFloat(tokens[3]);
            return new G.GRect({ x: x, y: y, width: width, height: height });
        };
        IDDSVGGraph.prototype.setViewBox = function (box) {
            var x = box.x;
            var y = box.y;
            var width = box.width;
            var height = box.height;
            this.chart.navigation.setVisibleRect({ x: x, y: -y - height, width: width, height: height }, false);
        };
        IDDSVGGraph.prototype.redrawGraph = function () {
            this.svg = this.gplot.svg;
            if (this.svg === undefined)
                return false;
            while (this.svg.childNodes.length > 0)
                this.svg.removeChild(this.svg.childNodes[0]);
            _super.prototype.populateGraph.call(this);
            if (this.graph == null)
                return false;
            return true;
        };
        IDDSVGGraph.prototype.drawGraph = function () {
            if (!this.redrawGraph())
                return;
            var bbox = this.graph.boundingBox;
            var offsetX = bbox.x;
            var offsetY = bbox.y;
            var cwidth = parseFloat(this.svg.getAttribute('width'));
            var cheight = parseFloat(this.svg.getAttribute('height'));
            var scaleX = cwidth / bbox.width;
            var scaleY = cheight / bbox.height;
            var scale = Math.min(scaleX, scaleY);
            this.chart.navigation.setVisibleRect({ x: offsetX, y: -offsetY - (isNaN(scale) ? bbox.height : (cheight / scale)), width: bbox.width, height: bbox.height }, false);
        };
        IDDSVGGraph.prototype.disableIDDMouseHandling = function () {
            this.gestureSource = this.chart.navigation.gestureSource;
            this.chart.navigation.gestureSource = undefined;
        };
        IDDSVGGraph.prototype.restoreIDDMouseHandling = function () {
            if (this.gestureSource != null)
                this.chart.navigation.gestureSource = this.gestureSource;
        };
        IDDSVGGraph.prototype.resetZoomLevel = function () {
            this.referenceScale = this.chart.navigation.widthScale;
        };
        IDDSVGGraph.prototype.getZoomLevel = function () {
            return this.chart.navigation.widthScale / this.referenceScale;
        };
        IDDSVGGraph.prototype.setZoomLevel = function (zoomLevel) {
            this.chart.navigation.widthScale = this.referenceScale * zoomLevel;
        };
        IDDSVGGraph.prototype.hookUpMouseEvents = function () {
            _super.prototype.hookUpMouseEvents.call(this);
            var that = this;
            this.container.onmousedown = function (e) {
                if (that.allowEditing && that.getObjectUnderMouseCursor() != null)
                    that.disableIDDMouseHandling();
                that.onMouseDown(e);
            };
            this.container.onmouseup = function (e) {
                that.onMouseUp(e);
                if (that.allowEditing)
                    that.restoreIDDMouseHandling();
            };
        };
        IDDSVGGraph.prototype.onMouseDblClick = function (e) {
            if (this.svg != null && !this.isEditingEdge())
                this.setViewBox(this.graph.boundingBox);
            else
                _super.prototype.onMouseDblClick.call(this, e);
        };
        IDDSVGGraph.msaglPlot = function (graph, jqDiv, master) {
            this.base = InteractiveDataDisplay.Plot;
            this.base(jqDiv, master);
            var that = this;
            var _svgCnt = undefined;
            var _svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
            Object.defineProperty(this, "svg", {
                get: function () {
                    return _svg;
                },
            });
            this.computeLocalBounds = function (step, computedBounds) {
                if (graph.graph == null)
                    return undefined;
                return { x: graph.graph.boundingBox.x, y: (-graph.graph.boundingBox.y - graph.graph.boundingBox.height), width: graph.graph.boundingBox.width, height: graph.graph.boundingBox.height };
            };
            this.arrange = function (finalRect) {
                InteractiveDataDisplay.CanvasPlot.prototype.arrange.call(this, finalRect);
                if (_svgCnt === undefined) {
                    _svgCnt = $("<div></div>").css("overflow", "hidden").appendTo(that.host)[0];
                    _svg.setAttribute("preserveAspectRatio", "xMinYMin slice");
                    _svgCnt.appendChild(_svg);
                }
                _svg.setAttribute("width", finalRect.width);
                _svg.setAttribute("height", finalRect.height);
                _svgCnt.setAttribute("width", finalRect.width);
                _svgCnt.setAttribute("height", finalRect.height);
                if (_svg !== undefined) {
                    var plotRect = that.visibleRect;
                    if (!isNaN(plotRect.y) && !isNaN(plotRect.height)) {
                        var zoom = finalRect.width / plotRect.width;
                        if (plotRect.width > 0 && plotRect.height > 0)
                            _svg.setAttribute("viewBox", plotRect.x + " " + (-plotRect.y - plotRect.height) + " " + plotRect.width + " " + plotRect.height);
                    }
                }
            };
        };
        return IDDSVGGraph;
    }(SVGGraph));
    return IDDSVGGraph;
});
//# sourceMappingURL=iddsvggraph.js.map