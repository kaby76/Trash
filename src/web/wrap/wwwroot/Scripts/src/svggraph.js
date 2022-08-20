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
define(["require", "exports", "./ggraph", "filesaver"], function (require, exports, G) {
    "use strict";
    var RenderElement = (function () {
        function RenderElement(group) {
            this.group = group;
        }
        return RenderElement;
    }());
    var RenderNode = (function (_super) {
        __extends(RenderNode, _super);
        function RenderNode(node, group) {
            var _this = _super.call(this, group) || this;
            _this.node = node;
            return _this;
        }
        RenderNode.prototype.getGeometryElement = function () { return this.node; };
        return RenderNode;
    }(RenderElement));
    var RenderEdge = (function (_super) {
        __extends(RenderEdge, _super);
        function RenderEdge(edge, group) {
            var _this = _super.call(this, group) || this;
            _this.edge = edge;
            return _this;
        }
        RenderEdge.prototype.getGeometryElement = function () { return this.edge; };
        return RenderEdge;
    }(RenderElement));
    var RenderEdgeLabel = (function (_super) {
        __extends(RenderEdgeLabel, _super);
        function RenderEdgeLabel(edge, group) {
            var _this = _super.call(this, group) || this;
            _this.edge = edge;
            return _this;
        }
        RenderEdgeLabel.prototype.getGeometryElement = function () { return this.edge.label; };
        return RenderEdgeLabel;
    }(RenderElement));
    var SVGGraph = (function () {
        function SVGGraph(container, graph) {
            this.grid = false;
            this.allowEditing = true;
            this.edgeRoutingCallback = null;
            this.layoutStartedCallback = null;
            this.workStoppedCallback = null;
            this.customDrawLabel = null;
            this.style = "text { stroke: black; fill: black; stroke-width: 0; font-size: 15px; font-family: Verdana, Arial, sans-serif }";
            this.onNodeClick = function (n) { };
            this.onEdgeClick = function (e) { };
            this.mousePoint = null;
            this.elementUnderMouseCursor = null;
            this.container = container;
            this.container.style.position = "relative";
            this.graph = graph === undefined ? null : graph;
            var workingText = document.createTextNode("LAYOUT IN PROGRESS");
            var workingSpan = document.createElement("span");
            workingSpan.setAttribute("style", "position: absolute; top: 50%; width: 100%; text-align: center; z-index: 10");
            workingSpan.style.visibility = "hidden";
            workingSpan.appendChild(workingText);
            this.workingSpan = workingSpan;
            this.container.appendChild(this.workingSpan);
            this.hookUpMouseEvents();
        }
        SVGGraph.prototype.getGraph = function () { return this.graph; };
        SVGGraph.prototype.setGraph = function (graph) {
            var _this = this;
            if (this.graph != null) {
                this.graph.edgeRoutingCallbacks.remove(this.edgeRoutingCallback);
                this.graph.layoutStartedCallbacks.remove(this.layoutStartedCallback);
                this.graph.workStoppedCallbacks.remove(this.workStoppedCallback);
            }
            this.graph = graph;
            var that = this;
            this.edgeRoutingCallback = function (edges) {
                if (edges != null)
                    for (var e in edges)
                        that.redrawElement(that.renderEdges[edges[e]]);
            };
            this.graph.edgeRoutingCallbacks.add(this.edgeRoutingCallback);
            this.layoutStartedCallback = function () {
                if (_this.graph.nodes.length > 0)
                    that.workingSpan.style.visibility = "visible";
            };
            this.graph.layoutStartedCallbacks.add(this.layoutStartedCallback);
            this.workStoppedCallback = function () {
                that.workingSpan.style.visibility = "hidden";
            };
            this.graph.workStoppedCallbacks.add(this.workStoppedCallback);
        };
        SVGGraph.prototype.getSVGString = function () {
            if (this.svg == null)
                return null;
            var currentViewBox = this.svg.getAttribute("viewBox");
            var currentPreserve = this.svg.getAttribute("preserveAspectRatio");
            var bbox = this.graph.boundingBox;
            var offsetX = bbox.x;
            var offsetY = bbox.y;
            var maxViewBox = "" + offsetX + " " + offsetY + " " + bbox.width + " " + bbox.height;
            this.svg.setAttribute("viewBox", maxViewBox);
            this.svg.removeAttribute("preserveAspectRatio");
            var ret = (new XMLSerializer()).serializeToString(this.svg);
            this.svg.setAttribute("viewBox", currentViewBox);
            this.svg.setAttribute("preserveAspectRatio", currentPreserve);
            return ret;
        };
        SVGGraph.prototype.saveAsSVG = function (fileName) {
            fileName = fileName || "graph.svg";
            var svgString = this.getSVGString();
            var blob = new Blob([svgString], { type: "image/svg+xml" });
            saveAs(blob, fileName);
        };
        SVGGraph.prototype.pathEllipse = function (ellipse, continuous) {
            var center = ellipse.center;
            var yAxis = (ellipse.axisB.y == 0) ? ellipse.axisA.y : ellipse.axisB.y;
            var xAxis = (ellipse.axisA.x == 0) ? ellipse.axisB.x : ellipse.axisA.x;
            yAxis = Math.abs(yAxis);
            xAxis = Math.abs(xAxis);
            if (yAxis == 0 || xAxis == 0)
                return "";
            var counterClockwise = ellipse.axisA.x * ellipse.axisB.y - ellipse.axisB.x * ellipse.axisA.y > 0;
            var aHorz = ellipse.axisA.x != 0;
            var aPos = ellipse.axisA.x > 0 || ellipse.axisA.y > 0;
            var parStart = ellipse.parStart;
            var parEnd = ellipse.parEnd;
            var path = "";
            var isFullEllipse = Math.abs(Math.abs(parEnd - parStart) - 2 * Math.PI) < 0.01;
            if (isFullEllipse) {
                var firstHalf = new G.GEllipse(ellipse);
                var secondHalf = new G.GEllipse(ellipse);
                firstHalf.parEnd = (ellipse.parStart + ellipse.parEnd) / 2;
                secondHalf.parStart = (ellipse.parStart + ellipse.parEnd) / 2;
                path += this.pathEllipse(firstHalf, continuous);
                path += this.pathEllipse(secondHalf, true);
            }
            else {
                var rots = aHorz ? aPos ? 0 : 2 : (aPos == counterClockwise) ? 1 : 3;
                parStart += Math.PI * rots / 2;
                parEnd += Math.PI * rots / 2;
                if (!counterClockwise) {
                    parStart = -parStart;
                    parEnd = -parEnd;
                }
                var startX = center.x + xAxis * Math.cos(parStart);
                var startY = center.y + yAxis * Math.sin(parStart);
                var endX = center.x + xAxis * Math.cos(parEnd);
                var endY = center.y + yAxis * Math.sin(parEnd);
                var largeArc = Math.abs(parEnd - parStart) > Math.PI;
                var sweepFlag = counterClockwise;
                path += (continuous ? " L" : " M") + startX + " " + startY;
                path += " A" + xAxis + " " + yAxis;
                path += " 0";
                path += largeArc ? " 1" : " 0";
                path += sweepFlag ? " 1" : " 0";
                path += " " + endX + " " + endY;
            }
            return path;
        };
        SVGGraph.prototype.pathLine = function (line, continuous) {
            var start = line.start;
            var end = line.end;
            var path = continuous ? "" : (" M" + start.x + " " + start.y);
            path += " L" + end.x + " " + end.y;
            return path;
        };
        SVGGraph.prototype.pathBezier = function (bezier, continuous) {
            var start = bezier.start;
            var p1 = bezier.p1;
            var p2 = bezier.p2;
            var p3 = bezier.p3;
            var path = (continuous ? " L" : " M") + start.x + " " + start.y;
            path += " C" + p1.x + " " + p1.y + " " + p2.x + " " + p2.y + " " + p3.x + " " + p3.y;
            return path;
        };
        SVGGraph.prototype.pathSegmentedCurve = function (curve, continuous) {
            var path = "";
            for (var i = 0; i < curve.segments.length; i++)
                path += this.pathCurve(curve.segments[i], continuous || path != "");
            return path;
        };
        SVGGraph.prototype.pathPolyline = function (polyline, continuous) {
            var start = polyline.start;
            var path = " M" + start.x + " " + start.y;
            for (var i = 0; i < polyline.points.length; i++) {
                var point = polyline.points[i];
                path += " L" + point.x + " " + point.y;
            }
            if (polyline.closed)
                path + " F";
            return path;
        };
        SVGGraph.prototype.pathRoundedRect = function (roundedRect, continuous) {
            var curve = roundedRect.getCurve();
            return this.pathSegmentedCurve(curve, continuous);
        };
        SVGGraph.prototype.pathCurve = function (curve, continuous) {
            if (curve == null)
                return "";
            if (curve.curvetype === "SegmentedCurve")
                return this.pathSegmentedCurve(curve, continuous);
            else if (curve.curvetype === "Polyline")
                return this.pathPolyline(curve, continuous);
            else if (curve.curvetype === "Bezier")
                return this.pathBezier(curve, continuous);
            else if (curve.curvetype === "Line")
                return this.pathLine(curve, continuous);
            else if (curve.curvetype === "Ellipse")
                return this.pathEllipse(curve, continuous);
            else if (curve.curvetype === "RoundedRect")
                return this.pathRoundedRect(curve, continuous);
            else
                throw "unknown curve type: " + curve.curvetype;
        };
        SVGGraph.prototype.drawLabel = function (parent, label, owner) {
            var g = document.createElementNS(SVGGraph.SVGNS, "g");
            if (this.customDrawLabel == null || !this.customDrawLabel(this.svg, parent, label, owner)) {
                var text = document.createElementNS(SVGGraph.SVGNS, "text");
                text.setAttribute("x", label.bounds.x.toString());
                text.setAttribute("y", (label.bounds.y + label.bounds.height).toString());
                text.textContent = label.content;
                text.setAttribute("style", "fill: " + (label.fill == "" ? "black" : label.fill + "; text-anchor: start"));
                g.appendChild(text);
            }
            parent.appendChild(g);
            if (owner instanceof G.GEdge) {
                var edge = owner;
                if (this.renderEdgeLabels[edge.id] == null)
                    this.renderEdgeLabels[edge.id] = new RenderEdgeLabel(edge, g);
                var renderLabel = this.renderEdgeLabels[edge.id];
                this.renderEdgeLabels[edge.id].group = g;
                var that = this;
                g.onmouseover = function (e) { that.onEdgeLabelMouseOver(renderLabel, e); };
                g.onmouseout = function (e) { that.onEdgeLabelMouseOut(renderLabel, e); };
            }
        };
        SVGGraph.prototype.drawNode = function (parent, node) {
            var g = document.createElementNS(SVGGraph.SVGNS, "g");
            var nodeCopy = node;
            var that = this;
            g.onclick = function () { that.onNodeClick(nodeCopy); };
            var curve = node.boundaryCurve;
            var pathString = this.pathCurve(curve, false) + "Z";
            var pathStyle = "stroke: " + node.stroke + "; fill: " + (node.fill == "" ? "none" : node.fill) + "; stroke-width: " + node.thickness + "; stroke-linejoin: miter; stroke-miterlimit: 2.0";
            if (node.shape != null && node.shape.multi > 0) {
                var path = document.createElementNS(SVGGraph.SVGNS, "path");
                path.setAttribute("d", pathString);
                path.setAttribute("transform", "translate(5,5)");
                path.setAttribute("style", pathStyle);
                g.appendChild(path);
            }
            var path = document.createElementNS(SVGGraph.SVGNS, "path");
            path.setAttribute("d", pathString);
            path.setAttribute("style", pathStyle);
            g.appendChild(path);
            if (node.label !== null)
                this.drawLabel(g, node.label, node);
            if (node.tooltip != null) {
                var title = document.createElementNS(SVGGraph.SVGNS, "title");
                title.textContent = node.tooltip;
                g.appendChild(title);
            }
            parent.appendChild(g);
            if (this.renderNodes[node.id] == null)
                this.renderNodes[node.id] = new RenderNode(node, g);
            this.renderNodes[node.id].group = g;
            var renderNode = this.renderNodes[node.id];
            g.onclick = function () { that.onNodeClick(renderNode.node); };
            g.onmouseover = function (e) { that.onNodeMouseOver(renderNode, e); };
            g.onmouseout = function (e) { that.onNodeMouseOut(renderNode, e); };
            var cluster = node;
            if (cluster.children !== undefined)
                for (var i = 0; i < cluster.children.length; i++)
                    this.drawNode(parent, cluster.children[i]);
        };
        SVGGraph.prototype.drawArrow = function (parent, arrowHead, style) {
            var start = arrowHead.start;
            var end = arrowHead.end;
            if (start == null || end == null)
                return;
            var dir = new G.GPoint({ x: start.x - end.x, y: start.y - end.y });
            var offsetX = -dir.y * Math.tan(25 * 0.5 * (Math.PI / 180));
            var offsetY = dir.x * Math.tan(25 * 0.5 * (Math.PI / 180));
            var pathString = "";
            if (arrowHead.style == "tee") {
                pathString += " M" + (start.x + offsetX) + " " + (start.y + offsetY);
                pathString += " L" + (start.x - offsetX) + " " + (start.y - offsetY);
            }
            else if (arrowHead.style == "diamond") {
                pathString += " M" + (start.x) + " " + (start.y);
                pathString += " L" + (start.x - (offsetX + dir.x / 2)) + " " + (start.y - (offsetY + dir.y / 2));
                pathString += " L" + (end.x) + " " + (end.y);
                pathString += " L" + (start.x - (-offsetX + dir.x / 2)) + " " + (start.y - (-offsetY + dir.y / 2));
                pathString += " Z";
            }
            else {
                pathString += " M" + (start.x + offsetX) + " " + (start.y + offsetY);
                pathString += " L" + end.x + " " + end.y;
                pathString += " L" + (start.x - offsetX) + " " + (start.y - offsetY);
                if (arrowHead.closed)
                    pathString += " Z";
                else {
                    pathString += " M" + start.x + " " + start.y;
                    pathString += " L" + end.x + " " + end.y;
                }
            }
            var path = document.createElementNS(SVGGraph.SVGNS, "path");
            path.setAttribute("d", pathString);
            if (arrowHead.dash != null)
                style += "; stroke-dasharray: " + arrowHead.dash;
            path.setAttribute("style", style);
            parent.appendChild(path);
        };
        SVGGraph.prototype.drawEdge = function (parent, edge) {
            var curve = edge.curve;
            if (curve == null) {
                console.log("MSAGL warning: did not receive a curve for edge " + edge.id);
                return;
            }
            var g = document.createElementNS(SVGGraph.SVGNS, "g");
            var edgeCopy = edge;
            var that = this;
            g.onclick = function () { that.onEdgeClick(edgeCopy); };
            var pathString = this.pathCurve(curve, false);
            var path = document.createElementNS(SVGGraph.SVGNS, "path");
            path.setAttribute("d", pathString);
            var style = "stroke: " + edge.stroke + "; stroke-width: " + edge.thickness + "; fill: none";
            if (edge.dash != null)
                style += "; stroke-dasharray: " + edge.dash;
            path.setAttribute("style", style);
            g.appendChild(path);
            if (edge.arrowHeadAtTarget != null)
                this.drawArrow(g, edge.arrowHeadAtTarget, "stroke: " + edge.stroke + "; stroke-width: " + edge.thickness + "; fill: " + (edge.arrowHeadAtTarget.fill ? edge.stroke : "none"));
            if (edge.arrowHeadAtSource != null)
                this.drawArrow(g, edge.arrowHeadAtSource, "stroke: " + edge.stroke + "; stroke-width: " + edge.thickness + "; fill: " + (edge.arrowHeadAtSource.fill ? edge.stroke : "none"));
            if (edge.label != null)
                this.drawLabel(this.svg, edge.label, edge);
            if (edge.tooltip != null) {
                var title = document.createElementNS(SVGGraph.SVGNS, "title");
                title.textContent = edge.tooltip;
                g.appendChild(title);
            }
            parent.appendChild(g);
            if (this.renderEdges[edge.id] == null)
                this.renderEdges[edge.id] = new RenderEdge(edge, g);
            var renderEdge = this.renderEdges[edge.id];
            renderEdge.group = g;
            g.onclick = function () { that.onEdgeClick(renderEdge.edge); };
            g.onmouseover = function (e) { that.onEdgeMouseOver(renderEdge, e); };
            g.onmouseout = function (e) { that.onEdgeMouseOut(renderEdge, e); };
        };
        SVGGraph.prototype.drawGrid = function (parent) {
            for (var x = 0; x < 10; x++)
                for (var y = 0; y < 10; y++) {
                    var circle = document.createElementNS(SVGGraph.SVGNS, "circle");
                    circle.setAttribute("r", "1");
                    circle.setAttribute("x", (x * 100).toString());
                    circle.setAttribute("y", (y * 100).toString());
                    circle.setAttribute("style", "fill: black; stroke: black; stroke-width: 1");
                    parent.appendChild(circle);
                }
        };
        SVGGraph.prototype.populateGraph = function () {
            if (this.style != null) {
                var style = document.createElementNS(SVGGraph.SVGNS, "style");
                var styleText = document.createTextNode(this.style);
                style.appendChild(styleText);
                this.svg.appendChild(style);
            }
            this.renderNodes = {};
            this.renderEdges = {};
            this.renderEdgeLabels = {};
            for (var i = 0; i < this.graph.nodes.length; i++)
                this.drawNode(this.svg, this.graph.nodes[i]);
            for (var i = 0; i < this.graph.edges.length; i++)
                this.drawEdge(this.svg, this.graph.edges[i]);
        };
        SVGGraph.prototype.drawGraph = function () {
            while (this.svg != null && this.svg.childElementCount > 0)
                this.svg.removeChild(this.svg.firstChild);
            if (this.grid)
                this.drawGrid(this.svg);
            if (this.graph == null)
                return;
            if (this.svg == null) {
                this.svg = document.createElementNS(SVGGraph.SVGNS, "svg");
                this.container.appendChild(this.svg);
            }
            var bbox = this.graph.boundingBox;
            var offsetX = bbox.x;
            var offsetY = bbox.y;
            var width = this.container.offsetWidth;
            var height = this.container.offsetHeight;
            var viewBox = "" + offsetX + " " + offsetY + " " + bbox.width + " " + bbox.height;
            this.svg.setAttribute("viewBox", viewBox);
            this.populateGraph();
        };
        SVGGraph.prototype.hookUpMouseEvents = function () {
            var that = this;
            this.container.onmousemove = function (e) { that.onMouseMove(e); };
            this.container.onmouseleave = function (e) { that.onMouseOut(e); };
            this.container.onmousedown = function (e) { that.onMouseDown(e); };
            this.container.onmouseup = function (e) { that.onMouseUp(e); };
            this.container.ondblclick = function (e) { that.onMouseDblClick(e); };
        };
        SVGGraph.prototype.containsGroup = function (g) {
            if (this.svg.contains != null)
                return this.svg.contains(g);
            for (var i = 0; i < this.svg.childNodes.length; i++)
                if (this.svg.childNodes[i] == g)
                    return true;
            return false;
        };
        SVGGraph.prototype.redrawElement = function (el) {
            if (el instanceof RenderNode) {
                var renderNode = el;
                if (this.containsGroup(renderNode.group))
                    this.svg.removeChild(renderNode.group);
                this.drawNode(this.svg, renderNode.node);
            }
            else if (el instanceof RenderEdge) {
                var renderEdge = el;
                if (this.containsGroup(renderEdge.group))
                    this.svg.removeChild(renderEdge.group);
                var renderLabel = this.renderEdgeLabels[renderEdge.edge.id];
                if (renderLabel != null)
                    this.svg.removeChild(renderLabel.group);
                this.drawEdge(this.svg, renderEdge.edge);
                if (this.edgeEditEdge == renderEdge)
                    this.drawPolylineCircles();
            }
            else if (el instanceof RenderEdgeLabel) {
                var renderEdgeLabel = el;
                if (this.containsGroup(renderEdgeLabel.group))
                    this.svg.removeChild(renderEdgeLabel.group);
                this.drawLabel(this.svg, renderEdgeLabel.edge.label, renderEdgeLabel.edge);
            }
        };
        SVGGraph.prototype.getMousePoint = function () { return this.mousePoint; };
        ;
        SVGGraph.prototype.getObjectUnderMouseCursor = function () {
            return this.elementUnderMouseCursor == null ? null : this.elementUnderMouseCursor.getGeometryElement();
        };
        ;
        SVGGraph.prototype.getGraphPoint = function (e) {
            var clientPoint = this.svg.createSVGPoint();
            clientPoint.x = e.clientX;
            clientPoint.y = e.clientY;
            var matrix = this.svg.getScreenCTM().inverse();
            var graphPoint = clientPoint.matrixTransform(matrix);
            return new G.GPoint({ x: graphPoint.x, y: graphPoint.y });
        };
        ;
        SVGGraph.prototype.onMouseMove = function (e) {
            if (this.svg == null)
                return;
            this.mousePoint = this.getGraphPoint(e);
            this.doDrag();
        };
        ;
        SVGGraph.prototype.onMouseOut = function (e) {
            if (this.svg == null)
                return;
            this.mousePoint = null;
            this.elementUnderMouseCursor = null;
            this.endDrag();
        };
        ;
        SVGGraph.prototype.onMouseDown = function (e) {
            if (this.svg == null)
                return;
            this.mouseDownPoint = new G.GPoint(this.getGraphPoint(e));
            if (this.allowEditing)
                this.beginDrag();
        };
        ;
        SVGGraph.prototype.onMouseUp = function (e) {
            if (this.svg == null)
                return;
            this.endDrag();
        };
        ;
        SVGGraph.prototype.onMouseDblClick = function (e) {
            if (this.svg == null)
                return;
            if (this.edgeEditEdge != null)
                this.edgeControlPointEvent(this.getGraphPoint(e));
        };
        SVGGraph.prototype.onNodeMouseOver = function (n, e) {
            if (this.svg == null)
                return;
            this.elementUnderMouseCursor = n;
        };
        ;
        SVGGraph.prototype.onNodeMouseOut = function (n, e) {
            if (this.svg == null)
                return;
            this.elementUnderMouseCursor = null;
        };
        ;
        SVGGraph.prototype.onEdgeMouseOver = function (ed, e) {
            if (this.svg == null)
                return;
            this.elementUnderMouseCursor = ed;
            if (this.allowEditing)
                this.enterEdgeEditMode(ed);
        };
        ;
        SVGGraph.prototype.onEdgeMouseOut = function (ed, e) {
            if (this.svg == null)
                return;
            this.beginExitEdgeEditMode();
            this.elementUnderMouseCursor = null;
        };
        ;
        SVGGraph.prototype.onEdgeLabelMouseOver = function (l, e) {
            if (this.svg == null)
                return;
            this.elementUnderMouseCursor = l;
        };
        ;
        SVGGraph.prototype.onEdgeLabelMouseOut = function (l, e) {
            if (this.svg == null)
                return;
            this.elementUnderMouseCursor = null;
        };
        ;
        SVGGraph.prototype.getDragObject = function () { return this.dragElement == null ? null : this.dragElement.getGeometryElement(); };
        ;
        SVGGraph.prototype.beginDrag = function () {
            if (this.elementUnderMouseCursor == null)
                return;
            var geometryElement = this.elementUnderMouseCursor.getGeometryElement();
            this.graph.startMoveElement(geometryElement, this.mouseDownPoint);
            this.dragElement = this.elementUnderMouseCursor;
        };
        ;
        SVGGraph.prototype.doDrag = function () {
            if (this.dragElement == null)
                return;
            var delta = this.mousePoint.sub(this.mouseDownPoint);
            this.graph.moveElements(delta);
            this.redrawElement(this.dragElement);
        };
        ;
        SVGGraph.prototype.endDrag = function () {
            this.graph.endMoveElements();
            this.dragElement = null;
        };
        ;
        SVGGraph.prototype.isEditingEdge = function () {
            return this.edgeEditEdge != null;
        };
        SVGGraph.prototype.drawPolylineCircles = function () {
            if (this.edgeEditEdge == null)
                return;
            var group = this.edgeEditEdge.group;
            var points = this.graph.getPolyline(this.edgeEditEdge.edge.id);
            var existingCircles = [];
            for (var i = 0; i < group.childNodes.length; i++)
                if (group.childNodes[i].nodeName == "circle")
                    existingCircles.push(group.childNodes[i]);
            for (var i = 0; i < points.length; i++) {
                var point = points[i];
                var c = i < existingCircles.length ? existingCircles[i] : document.createElementNS(SVGGraph.SVGNS, "circle");
                c.setAttribute("r", G.GGraph.EdgeEditCircleRadius.toString());
                c.setAttribute("cx", point.x.toString());
                c.setAttribute("cy", point.y.toString());
                c.setAttribute("style", "stroke: #5555FF; stroke-width: 1px; fill: transparent");
                if (i >= existingCircles.length)
                    group.insertBefore(c, group.childNodes[0]);
            }
            for (var i = points.length; i < existingCircles.length; i++)
                group.removeChild(existingCircles[i]);
        };
        SVGGraph.prototype.clearPolylineCircles = function () {
            if (this.edgeEditEdge == null)
                return;
            var circles = [];
            var group = this.edgeEditEdge.group;
            for (var i = 0; i < group.childNodes.length; i++)
                if (group.childNodes[i].nodeName == "circle")
                    circles.push(group.childNodes[i]);
            for (var i = 0; i < circles.length; i++)
                group.removeChild(circles[i]);
        };
        SVGGraph.prototype.enterEdgeEditMode = function (ed) {
            if (this.edgeEditEdge == ed) {
                clearTimeout(this.edgeEditModeTimeout);
                this.edgeEditModeTimeout = 0;
            }
            if (this.edgeEditEdge != null && this.edgeEditEdge != ed)
                return;
            this.edgeEditEdge = ed;
            this.drawPolylineCircles();
        };
        SVGGraph.prototype.exitEdgeEditMode = function () {
            var ed = this.edgeEditEdge;
            if (ed == null)
                return;
            clearTimeout(this.edgeEditModeTimeout);
            this.clearPolylineCircles();
            this.edgeEditModeTimeout = 0;
            this.edgeEditEdge = null;
        };
        SVGGraph.prototype.beginExitEdgeEditMode = function () {
            var that = this;
            this.edgeEditModeTimeout = setTimeout(function () { return that.exitEdgeEditMode(); }, SVGGraph.ExitEdgeModeTimeout);
        };
        SVGGraph.prototype.edgeControlPointEvent = function (point) {
            var clickedPoint = this.graph.getControlPointAt(this.edgeEditEdge.edge, point);
            if (clickedPoint != null) {
                this.graph.delEdgeControlPoint(this.edgeEditEdge.edge.id, clickedPoint);
                this.elementUnderMouseCursor = null;
                this.beginExitEdgeEditMode();
            }
            else {
                this.graph.addEdgeControlPoint(this.edgeEditEdge.edge.id, point);
                clearTimeout(this.edgeEditModeTimeout);
                this.edgeEditModeTimeout = 0;
                this.elementUnderMouseCursor = this.edgeEditEdge;
            }
        };
        SVGGraph.SVGNS = "http://www.w3.org/2000/svg";
        SVGGraph.ExitEdgeModeTimeout = 2000;
        return SVGGraph;
    }());
    return SVGGraph;
});
//# sourceMappingURL=svggraph.js.map