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
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.GGraph = exports.CallbackSet = exports.GSettings = exports.GUpDownConstraint = exports.GPlaneTransformation = exports.GEdge = exports.GArrowHead = exports.GCluster = exports.GClusterMargin = exports.GNode = exports.GShape = exports.GLabel = exports.GSegmentedCurve = exports.GBezier = exports.GRoundedRect = exports.GPolyline = exports.GLine = exports.GEllipse = exports.GCurve = exports.GRect = exports.GPoint = void 0;
    var GPoint = (function () {
        function GPoint(p) {
            this.x = p.x === undefined ? 0 : p.x;
            this.y = p.y === undefined ? 0 : p.y;
        }
        GPoint.prototype.equals = function (other) {
            return this.x == other.x && this.y == other.y;
        };
        GPoint.prototype.add = function (other) {
            return new GPoint({ x: this.x + other.x, y: this.y + other.y });
        };
        GPoint.prototype.sub = function (other) {
            return new GPoint({ x: this.x - other.x, y: this.y - other.y });
        };
        GPoint.prototype.div = function (op) {
            return new GPoint({ x: this.x / op, y: this.y / op });
        };
        GPoint.prototype.mul = function (op) {
            return new GPoint({ x: this.x * op, y: this.y * op });
        };
        GPoint.prototype.vmul = function (other) {
            return this.x * other.x + this.y * other.y;
        };
        GPoint.prototype.dist2 = function (other) {
            var d = this.sub(other);
            return d.x * d.x + d.y * d.y;
        };
        GPoint.prototype.closestParameter = function (start, end) {
            var bc = end.sub(start);
            var ba = this.sub(start);
            var c1 = bc.vmul(ba);
            if (c1 <= 0.1)
                return 0;
            var c2 = bc.vmul(bc);
            if (c2 <= c1 + 0.1)
                return 1;
            return c1 / c2;
        };
        GPoint.signedDoubledTriangleArea = function (cornerA, cornerB, cornerC) {
            return (cornerB.x - cornerA.x) * (cornerC.y - cornerA.y) - (cornerC.x - cornerA.x) * (cornerB.y - cornerA.y);
        };
        GPoint.origin = new GPoint({ x: 0, y: 0 });
        return GPoint;
    }());
    exports.GPoint = GPoint;
    var GRect = (function () {
        function GRect(r) {
            this.x = r.x === undefined ? 0 : r.x;
            this.y = r.y === undefined ? 0 : r.y;
            this.width = r.width === undefined ? 0 : r.width;
            this.height = r.height === undefined ? 0 : r.height;
        }
        GRect.prototype.getTopLeft = function () {
            return new GPoint({ x: this.x, y: this.y });
        };
        GRect.prototype.getBottomRight = function () {
            return new GPoint({ x: this.getRight(), y: this.getBottom() });
        };
        GRect.prototype.getBottom = function () {
            return this.y + this.height;
        };
        GRect.prototype.getRight = function () {
            return this.x + this.width;
        };
        GRect.prototype.getCenter = function () {
            return new GPoint({ x: this.x + this.width / 2, y: this.y + this.height / 2 });
        };
        GRect.prototype.setCenter = function (p) {
            var delta = p.sub(this.getCenter());
            this.x += delta.x;
            this.y += delta.y;
        };
        GRect.prototype.extend = function (other) {
            if (other == null)
                return this;
            return new GRect({
                x: Math.min(this.x, other.x),
                y: Math.min(this.y, other.y),
                width: Math.max(this.getRight(), other.getRight()) - Math.min(this.x, other.x),
                height: Math.max(this.getBottom(), other.getBottom()) - Math.min(this.y, other.y)
            });
        };
        GRect.prototype.extendP = function (point) {
            return this.extend(new GRect({ x: point.x, y: point.y, width: 0, height: 0 }));
        };
        GRect.zero = new GRect({ x: 0, y: 0, width: 0, height: 0 });
        return GRect;
    }());
    exports.GRect = GRect;
    var GCurve = (function () {
        function GCurve(curvetype) {
            if (curvetype === undefined)
                throw new Error("Undefined curve type");
            this.curvetype = curvetype;
        }
        GCurve.ofCurve = function (curve) {
            if (curve == null || curve === undefined)
                return null;
            var ret;
            if (curve.curvetype == "Ellipse")
                ret = new GEllipse(curve);
            else if (curve.curvetype == "Line")
                ret = new GLine(curve);
            else if (curve.curvetype == "Bezier")
                ret = new GBezier(curve);
            else if (curve.curvetype == "Polyline")
                ret = new GPolyline(curve);
            else if (curve.curvetype == "SegmentedCurve")
                ret = new GSegmentedCurve(curve);
            else if (curve.curvetype == "RoundedRect")
                ret = new GRoundedRect(curve);
            return ret;
        };
        return GCurve;
    }());
    exports.GCurve = GCurve;
    var GEllipse = (function (_super) {
        __extends(GEllipse, _super);
        function GEllipse(ellipse) {
            var _this = _super.call(this, "Ellipse") || this;
            _this.center = ellipse.center === undefined ? GPoint.origin : new GPoint(ellipse.center);
            _this.axisA = ellipse.axisA === undefined ? GPoint.origin : new GPoint(ellipse.axisA);
            _this.axisB = ellipse.axisB === undefined ? GPoint.origin : new GPoint(ellipse.axisB);
            _this.parStart = ellipse.parStart === undefined ? 0 : ellipse.parStart;
            _this.parEnd = ellipse.parStart === undefined ? Math.PI * 2 : ellipse.parEnd;
            return _this;
        }
        GEllipse.prototype.getCenter = function () {
            return this.center;
        };
        GEllipse.prototype.getStart = function () {
            return this.center.add(this.axisA.mul(Math.cos(this.parStart))).add(this.axisB.mul(Math.sin(this.parStart)));
        };
        GEllipse.prototype.getEnd = function () {
            return this.center.add(this.axisA.mul(Math.cos(this.parEnd))).add(this.axisB.mul(Math.sin(this.parEnd)));
        };
        GEllipse.prototype.setCenter = function (p) {
            this.center = new GPoint(p);
        };
        GEllipse.prototype.getBoundingBox = function () {
            var width = 2 * Math.max(Math.abs(this.axisA.x), Math.abs(this.axisB.x));
            var height = 2 * Math.max(Math.abs(this.axisA.y), Math.abs(this.axisB.y));
            var p = this.center.sub({ x: width / 2, y: height / 2 });
            return new GRect({ x: p.x, y: p.y, width: width, height: height });
        };
        GEllipse.make = function (width, height) {
            return new GEllipse({ center: GPoint.origin, axisA: new GPoint({ x: width / 2, y: 0 }), axisB: new GPoint({ x: 0, y: height / 2 }), parStart: 0, parEnd: Math.PI * 2 });
        };
        return GEllipse;
    }(GCurve));
    exports.GEllipse = GEllipse;
    var GLine = (function (_super) {
        __extends(GLine, _super);
        function GLine(line) {
            var _this = _super.call(this, "Line") || this;
            _this.start = line.start === undefined ? GPoint.origin : new GPoint(line.start);
            _this.end = line.end === undefined ? GPoint.origin : new GPoint(line.end);
            return _this;
        }
        GLine.prototype.getCenter = function () {
            return this.start.add(this.end).div(2);
        };
        GLine.prototype.getStart = function () {
            return this.start;
        };
        GLine.prototype.getEnd = function () {
            return this.end;
        };
        GLine.prototype.setCenter = function (p) {
            var delta = p.sub(this.getCenter());
            this.start = this.start.add(delta);
            this.end = this.end.add(delta);
        };
        GLine.prototype.getBoundingBox = function () {
            var ret = new GRect({ x: this.start.x, y: this.start.y, width: 0, height: 0 });
            ret = ret.extendP(this.end);
            return ret;
        };
        return GLine;
    }(GCurve));
    exports.GLine = GLine;
    var GPolyline = (function (_super) {
        __extends(GPolyline, _super);
        function GPolyline(polyline) {
            var _this = _super.call(this, "Polyline") || this;
            _this.start = polyline.start === undefined ? GPoint.origin : new GPoint(polyline.start);
            _this.points = [];
            for (var i = 0; i < polyline.points.length; i++)
                _this.points.push(new GPoint(polyline.points[i]));
            _this.closed = polyline.closed === undefined ? false : polyline.closed;
            return _this;
        }
        GPolyline.prototype.getCenter = function () {
            var ret = this.start;
            for (var i = 0; i < this.points.length; i++)
                ret = ret.add(this.points[i]);
            ret = ret.div(1 + this.points.length);
            return ret;
        };
        GPolyline.prototype.getStart = function () {
            return this.start;
        };
        GPolyline.prototype.getEnd = function () {
            return this.points[this.points.length - 1];
        };
        GPolyline.prototype.setCenter = function (p) {
            var delta = p.sub(this.getCenter());
            for (var i = 0; i < this.points.length; i++)
                this.points[i] = this.points[i].add(delta);
        };
        GPolyline.prototype.getBoundingBox = function () {
            var ret = new GRect({ x: this.points[0].x, y: this.points[0].y, height: 0, width: 0 });
            for (var i = 1; i < this.points.length; i++)
                ret = ret.extendP(this.points[i]);
            return ret;
        };
        return GPolyline;
    }(GCurve));
    exports.GPolyline = GPolyline;
    var GRoundedRect = (function (_super) {
        __extends(GRoundedRect, _super);
        function GRoundedRect(roundedRect) {
            var _this = _super.call(this, "RoundedRect") || this;
            _this.bounds = roundedRect.bounds === undefined ? GRect.zero : new GRect(roundedRect.bounds);
            _this.radiusX = roundedRect.radiusX === undefined ? 0 : roundedRect.radiusX;
            _this.radiusY = roundedRect.radiusY === undefined ? 0 : roundedRect.radiusY;
            return _this;
        }
        GRoundedRect.prototype.getCenter = function () {
            return this.bounds.getCenter();
        };
        GRoundedRect.prototype.getStart = function () {
            throw new Error("getStart not supported by RoundedRect");
        };
        GRoundedRect.prototype.getEnd = function () {
            throw new Error("getEnd not supported by RoundedRect");
        };
        GRoundedRect.prototype.setCenter = function (p) {
            this.bounds.setCenter(p);
        };
        GRoundedRect.prototype.getBoundingBox = function () {
            return this.bounds;
        };
        GRoundedRect.prototype.getCurve = function () {
            var segments = [];
            var axisA = new GPoint({ x: this.radiusX, y: 0 });
            var axisB = new GPoint({ x: 0, y: this.radiusY });
            var innerBounds = new GRect({ x: this.bounds.x + this.radiusX, y: this.bounds.y + this.radiusY, width: this.bounds.width - this.radiusX * 2, height: this.bounds.height - this.radiusY * 2 });
            segments.push(new GEllipse({ axisA: axisA, axisB: axisB, center: new GPoint({ x: innerBounds.x, y: innerBounds.y }), parStart: Math.PI, parEnd: Math.PI * 3 / 2 }));
            segments.push(new GLine({ start: new GPoint({ x: innerBounds.x, y: this.bounds.y }), end: new GPoint({ x: innerBounds.x + innerBounds.width, y: this.bounds.y }) }));
            segments.push(new GEllipse({ axisA: axisA, axisB: axisB, center: new GPoint({ x: innerBounds.x + innerBounds.width, y: innerBounds.y }), parStart: Math.PI * 3 / 2, parEnd: 2 * Math.PI }));
            segments.push(new GLine({ start: new GPoint({ x: this.bounds.x + this.bounds.width, y: innerBounds.y }), end: new GPoint({ x: this.bounds.x + this.bounds.width, y: innerBounds.y + innerBounds.height }) }));
            segments.push(new GEllipse({ axisA: axisA, axisB: axisB, center: new GPoint({ x: innerBounds.x + innerBounds.width, y: innerBounds.y + innerBounds.height }), parStart: 0, parEnd: Math.PI / 2 }));
            segments.push(new GLine({ start: new GPoint({ x: innerBounds.x + innerBounds.width, y: this.bounds.y + this.bounds.height }), end: new GPoint({ x: innerBounds.x, y: this.bounds.y + this.bounds.height }) }));
            segments.push(new GEllipse({ axisA: axisA, axisB: axisB, center: new GPoint({ x: innerBounds.x, y: innerBounds.y + innerBounds.height }), parStart: Math.PI / 2, parEnd: Math.PI }));
            segments.push(new GLine({ start: new GPoint({ x: this.bounds.x, y: innerBounds.y + innerBounds.height }), end: new GPoint({ x: this.bounds.x, y: innerBounds.y }) }));
            return new GSegmentedCurve({ segments: segments });
        };
        return GRoundedRect;
    }(GCurve));
    exports.GRoundedRect = GRoundedRect;
    var GBezier = (function (_super) {
        __extends(GBezier, _super);
        function GBezier(bezier) {
            var _this = _super.call(this, "Bezier") || this;
            _this.start = bezier.start === undefined ? GPoint.origin : new GPoint(bezier.start);
            _this.p1 = bezier.p1 === undefined ? GPoint.origin : new GPoint(bezier.p1);
            _this.p2 = bezier.p2 === undefined ? GPoint.origin : new GPoint(bezier.p2);
            _this.p3 = bezier.p3 === undefined ? GPoint.origin : new GPoint(bezier.p3);
            return _this;
        }
        GBezier.prototype.getCenter = function () {
            var ret = this.start;
            ret = ret.add(this.p1);
            ret = ret.add(this.p2);
            ret = ret.add(this.p3);
            ret = ret.div(4);
            return ret;
        };
        GBezier.prototype.getStart = function () {
            return this.start;
        };
        GBezier.prototype.getEnd = function () {
            return this.p3;
        };
        GBezier.prototype.setCenter = function (p) {
            var delta = p.sub(this.getCenter());
            this.start = this.start.add(delta);
            this.p1 = this.p1.add(delta);
            this.p2 = this.p2.add(delta);
            this.p3 = this.p3.add(delta);
        };
        GBezier.prototype.getBoundingBox = function () {
            var ret = new GRect({ x: this.start.x, y: this.start.y, width: 0, height: 0 });
            ret = ret.extendP(this.p1);
            ret = ret.extendP(this.p2);
            ret = ret.extendP(this.p3);
            return ret;
        };
        return GBezier;
    }(GCurve));
    exports.GBezier = GBezier;
    var GSegmentedCurve = (function (_super) {
        __extends(GSegmentedCurve, _super);
        function GSegmentedCurve(segmentedCurve) {
            var _this = _super.call(this, "SegmentedCurve") || this;
            _this.segments = [];
            for (var i = 0; i < segmentedCurve.segments.length; i++)
                _this.segments.push(GCurve.ofCurve(segmentedCurve.segments[i]));
            return _this;
        }
        GSegmentedCurve.prototype.getCenter = function () {
            var ret = GPoint.origin;
            for (var i = 0; i < this.segments.length; i++)
                ret = ret.add(this.segments[i].getCenter());
            ret = ret.div(this.segments.length);
            return ret;
        };
        GSegmentedCurve.prototype.getStart = function () {
            return this.segments[0].getStart();
        };
        GSegmentedCurve.prototype.getEnd = function () {
            return this.segments[this.segments.length - 1].getEnd();
        };
        GSegmentedCurve.prototype.setCenter = function (p) {
            throw new Error("setCenter not supported by SegmentedCurve");
        };
        GSegmentedCurve.prototype.getBoundingBox = function () {
            var ret = this.segments[0].getBoundingBox();
            for (var i = 1; i < this.segments.length; i++)
                ret = ret.extend(this.segments[i].getBoundingBox());
            return ret;
        };
        return GSegmentedCurve;
    }(GCurve));
    exports.GSegmentedCurve = GSegmentedCurve;
    var GLabel = (function () {
        function GLabel(label) {
            if (typeof (label) == "string")
                this.content = label;
            else {
                this.bounds = label.bounds == undefined || label.bounds == GRect.zero ? GRect.zero : new GRect(label.bounds);
                this.tooltip = label.tooltip === undefined ? null : label.tooltip;
                this.content = label.content;
                this.fill = label.fill === undefined ? "Black" : label.fill;
            }
        }
        return GLabel;
    }());
    exports.GLabel = GLabel;
    var GShape = (function () {
        function GShape() {
            this.radiusX = 0;
            this.radiusY = 0;
            this.multi = 0;
        }
        GShape.GetRect = function () {
            var ret = new GShape();
            ret.shape = "rect";
            return ret;
        };
        GShape.GetRoundedRect = function (radiusX, radiusY) {
            var ret = new GShape();
            ret.shape = "rect";
            ret.radiusX = radiusX === undefined ? 5 : radiusX;
            ret.radiusY = radiusY === undefined ? 5 : radiusY;
            return ret;
        };
        GShape.GetMaxRoundedRect = function () {
            var ret = new GShape();
            ret.shape = "rect";
            ret.radiusX = null;
            ret.radiusY = null;
            return ret;
        };
        GShape.FromString = function (shape) {
            if (shape == "rect")
                return GShape.GetRect();
            else if (shape == "roundedrect")
                return GShape.GetRoundedRect();
            else if (shape == "maxroundedrect")
                return GShape.GetMaxRoundedRect();
            return null;
        };
        GShape.RectShape = "rect";
        return GShape;
    }());
    exports.GShape = GShape;
    var GNode = (function () {
        function GNode(node) {
            if (node.id === undefined)
                throw new Error("Undefined node id");
            this.id = node.id;
            this.tooltip = node.tooltip === undefined ? null : node.tooltip;
            this.shape = node.shape === undefined ? null : typeof (node.shape) == "string" ? GShape.FromString(node.shape) : node.shape;
            this.boundaryCurve = GCurve.ofCurve(node.boundaryCurve);
            this.label = node.label === undefined ? null : node.label == null ? null : typeof (node.label) == "string" ? new GLabel({ content: node.label }) : new GLabel(node.label);
            this.labelMargin = node.labelMargin === undefined ? 5 : node.labelMargin;
            this.thickness = node.thickness == undefined ? 1 : node.thickness;
            this.fill = node.fill === undefined ? "White" : node.fill;
            this.stroke = node.stroke === undefined ? "Black" : node.stroke;
        }
        GNode.prototype.isCluster = function () {
            return this.children !== undefined;
        };
        return GNode;
    }());
    exports.GNode = GNode;
    var GClusterMargin = (function () {
        function GClusterMargin(clusterMargin) {
            this.bottom = clusterMargin.bottom == null ? 0 : clusterMargin.bottom;
            this.top = clusterMargin.top == null ? 0 : clusterMargin.top;
            this.left = clusterMargin.left == null ? 0 : clusterMargin.left;
            this.right = clusterMargin.right == null ? 0 : clusterMargin.right;
            this.minWidth = clusterMargin.minWidth == null ? 0 : clusterMargin.minWidth;
            this.minHeight = clusterMargin.minHeight == null ? 0 : clusterMargin.minHeight;
        }
        return GClusterMargin;
    }());
    exports.GClusterMargin = GClusterMargin;
    var GCluster = (function (_super) {
        __extends(GCluster, _super);
        function GCluster(cluster) {
            var _this = _super.call(this, cluster) || this;
            _this.margin = new GClusterMargin(cluster.margin == null ? {} : cluster.margin);
            _this.children = [];
            if (cluster.children != null)
                for (var i = 0; i < cluster.children.length; i++)
                    if (cluster.children[i].children !== undefined)
                        _this.children.push(new GCluster(cluster.children[i]));
                    else
                        _this.children.push(new GNode(cluster.children[i]));
            return _this;
        }
        GCluster.prototype.addChild = function (n) {
            this.children.push(n);
        };
        return GCluster;
    }(GNode));
    exports.GCluster = GCluster;
    var GArrowHead = (function () {
        function GArrowHead(arrowHead) {
            this.start = arrowHead.start == undefined ? null : arrowHead.start;
            this.end = arrowHead.end == undefined ? null : arrowHead.end;
            this.closed = arrowHead.closed == undefined ? false : arrowHead.closed;
            this.fill = arrowHead.fill == undefined ? false : arrowHead.fill;
            this.dash = arrowHead.dash == undefined ? null : arrowHead.dash;
            this.style = arrowHead.style == undefined ? "standard" : arrowHead.style;
        }
        GArrowHead.standard = new GArrowHead({});
        GArrowHead.closed = new GArrowHead({ closed: true });
        GArrowHead.filled = new GArrowHead({ closed: true, fill: true });
        GArrowHead.tee = new GArrowHead({ style: "tee" });
        GArrowHead.diamond = new GArrowHead({ style: "diamond" });
        GArrowHead.diamondFilled = new GArrowHead({ style: "diamond", fill: true });
        return GArrowHead;
    }());
    exports.GArrowHead = GArrowHead;
    var GEdge = (function () {
        function GEdge(edge) {
            if (edge.id === undefined)
                throw new Error("Undefined edge id");
            if (edge.source === undefined)
                throw new Error("Undefined edge source");
            if (edge.target === undefined)
                throw new Error("Undefined edge target");
            this.id = edge.id;
            this.tooltip = edge.tooltip === undefined ? null : edge.tooltip;
            this.source = edge.source;
            this.target = edge.target;
            this.label = edge.label === undefined || edge.label == null ? null : typeof (edge.label) == "string" ? new GLabel({ content: edge.label }) : new GLabel(edge.label);
            this.arrowHeadAtTarget = edge.arrowHeadAtTarget === undefined ? GArrowHead.standard : edge.arrowHeadAtTarget == null ? null : new GArrowHead(edge.arrowHeadAtTarget);
            this.arrowHeadAtSource = edge.arrowHeadAtSource === undefined || edge.arrowHeadAtSource == null ? null : new GArrowHead(edge.arrowHeadAtSource);
            this.thickness = edge.thickness == undefined ? 1 : edge.thickness;
            this.dash = edge.dash == undefined ? null : edge.dash;
            this.curve = edge.curve === undefined ? null : GCurve.ofCurve(edge.curve);
            this.stroke = edge.stroke === undefined ? "Black" : edge.stroke;
        }
        return GEdge;
    }());
    exports.GEdge = GEdge;
    var GPlaneTransformation = (function () {
        function GPlaneTransformation(transformation) {
            if (transformation.rotation !== undefined) {
                var angle = transformation.rotation;
                var cos = Math.cos(angle);
                var sin = Math.sin(angle);
                this.m00 = cos;
                this.m01 = -sin;
                this.m02 = 0;
                this.m10 = sin;
                this.m11 = cos;
                this.m12 = 0;
            }
            else {
                this.m00 = transformation.m00 === undefined ? -1 : transformation.m00;
                this.m01 = transformation.m01 === undefined ? -1 : transformation.m01;
                this.m02 = transformation.m02 === undefined ? -1 : transformation.m02;
                this.m10 = transformation.m10 === undefined ? -1 : transformation.m10;
                this.m11 = transformation.m11 === undefined ? -1 : transformation.m11;
                this.m12 = transformation.m12 === undefined ? -1 : transformation.m12;
            }
        }
        GPlaneTransformation.defaultTransformation = new GPlaneTransformation({ m00: -1, m01: 0, m02: 0, m10: 0, m11: -1, m12: 0 });
        GPlaneTransformation.ninetyDegreesTransformation = new GPlaneTransformation({ m00: 0, m01: -1, m02: 0, m10: 1, m11: 0, m12: 0 });
        return GPlaneTransformation;
    }());
    exports.GPlaneTransformation = GPlaneTransformation;
    var GUpDownConstraint = (function () {
        function GUpDownConstraint(upDownConstraint) {
            this.upNode = upDownConstraint.upNode;
            this.downNode = upDownConstraint.downNode;
        }
        return GUpDownConstraint;
    }());
    exports.GUpDownConstraint = GUpDownConstraint;
    var GSettings = (function () {
        function GSettings(settings) {
            this.layout = settings.layout === undefined ? GSettings.sugiyamaLayout : settings.layout;
            this.transformation = settings.transformation === undefined ? GPlaneTransformation.defaultTransformation : settings.transformation;
            this.routing = settings.routing === undefined ? GSettings.sugiyamaSplinesRouting : settings.routing;
            this.aspectRatio = settings.aspectRatio === undefined ? 0.0 : settings.aspectRatio;
            this.upDownConstraints = [];
            if (settings.upDownConstraints !== undefined) {
                for (var i = 0; i < settings.upDownConstraints.length; i++) {
                    var upDownConstraint = new GUpDownConstraint(settings.upDownConstraints[i]);
                    this.upDownConstraints.push(upDownConstraint);
                }
            }
            this.iterationsWithMajorization = settings.iterationsWithMajorization == undefined ? 30 : settings.iterationsWithMajorization;
        }
        GSettings.sugiyamaLayout = "sugiyama";
        GSettings.mdsLayout = "mds";
        GSettings.splinesRouting = "splines";
        GSettings.splinesBundlingRouting = "splinesbundling";
        GSettings.straightLineRouting = "straightline";
        GSettings.sugiyamaSplinesRouting = "sugiyamasplines";
        GSettings.rectilinearRouting = "rectilinear";
        GSettings.rectilinearToCenterRouting = "rectilineartocenter";
        return GSettings;
    }());
    exports.GSettings = GSettings;
    var GNodeInternal = (function () {
        function GNodeInternal() {
        }
        return GNodeInternal;
    }());
    var GEdgeInternal = (function () {
        function GEdgeInternal() {
        }
        return GEdgeInternal;
    }());
    var MoveElementToken = (function () {
        function MoveElementToken() {
        }
        return MoveElementToken;
    }());
    var MoveNodeToken = (function (_super) {
        __extends(MoveNodeToken, _super);
        function MoveNodeToken() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        return MoveNodeToken;
    }(MoveElementToken));
    var MoveEdgeLabelToken = (function (_super) {
        __extends(MoveEdgeLabelToken, _super);
        function MoveEdgeLabelToken() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        return MoveEdgeLabelToken;
    }(MoveElementToken));
    var MoveEdgeToken = (function (_super) {
        __extends(MoveEdgeToken, _super);
        function MoveEdgeToken() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        return MoveEdgeToken;
    }(MoveElementToken));
    var CallbackSet = (function () {
        function CallbackSet() {
            this.callbacks = [];
        }
        CallbackSet.prototype.add = function (callback) {
            if (callback == null)
                return;
            this.callbacks.push(callback);
        };
        CallbackSet.prototype.remove = function (callback) {
            if (callback == null)
                return;
            var idx = this.callbacks.indexOf(callback);
            if (idx >= 0)
                this.callbacks.splice(idx);
        };
        CallbackSet.prototype.fire = function (par) {
            for (var i = 0; i < this.callbacks.length; i++)
                this.callbacks[i](par);
        };
        CallbackSet.prototype.count = function () {
            return this.callbacks.length;
        };
        return CallbackSet;
    }());
    exports.CallbackSet = CallbackSet;
    var GGraph = (function () {
        function GGraph() {
            this.worker = null;
            this.working = false;
            this.layoutStartedCallbacks = new CallbackSet();
            this.layoutCallbacks = new CallbackSet();
            this.edgeRoutingCallbacks = new CallbackSet();
            this.workStartedCallbacks = new CallbackSet();
            this.workStoppedCallbacks = new CallbackSet();
            this.moveTokens = [];
            this.delayCheckRouteEdges = null;
            this.delayCheckRebuildEdge = null;
            this.nodesMap = {};
            this.edgesMap = {};
            this.nodes = [];
            this.edges = [];
            this.boundingBox = GRect.zero;
            this.settings = new GSettings({ transformation: { m00: -1, m01: 0, m02: 0, m10: 0, m11: -1, m12: 0 } });
        }
        GGraph.prototype.mapNode = function (node) {
            this.nodesMap[node.id] = { node: node, outEdges: [], inEdges: [], selfEdges: [] };
            var children = node.children;
            if (children !== undefined)
                for (var _i = 0, children_1 = children; _i < children_1.length; _i++) {
                    var child = children_1[_i];
                    this.mapNode(child);
                }
        };
        GGraph.prototype.addNode = function (node) {
            this.nodes.push(node);
            this.mapNode(node);
            var children = node.children;
            if (children !== undefined)
                for (var _i = 0, children_2 = children; _i < children_2.length; _i++) {
                    var child = children_2[_i];
                    this.mapNode(child);
                }
        };
        GGraph.prototype.getNode = function (id) {
            var nodeInternal = this.nodesMap[id];
            return nodeInternal == null ? null : nodeInternal.node;
        };
        GGraph.prototype.getInEdges = function (nodeId) {
            var nodeInternal = this.nodesMap[nodeId];
            return nodeInternal == null ? null : nodeInternal.inEdges;
        };
        GGraph.prototype.getOutEdges = function (nodeId) {
            var nodeInternal = this.nodesMap[nodeId];
            return nodeInternal == null ? null : nodeInternal.outEdges;
        };
        GGraph.prototype.getSelfEdges = function (nodeId) {
            var nodeInternal = this.nodesMap[nodeId];
            return nodeInternal == null ? null : nodeInternal.selfEdges;
        };
        GGraph.prototype.addEdge = function (edge) {
            if (this.nodesMap[edge.source] == null)
                throw new Error("Undefined node " + edge.source);
            if (this.nodesMap[edge.target] == null)
                throw new Error("Undefined node " + edge.target);
            if (this.edgesMap[edge.id] != null)
                throw new Error("Edge " + edge.id + " already exists");
            this.edgesMap[edge.id] = { edge: edge, polyline: null };
            this.edges.push(edge);
            if (edge.source == edge.target)
                this.nodesMap[edge.source].selfEdges.push(edge.id);
            else {
                this.nodesMap[edge.source].outEdges.push(edge.id);
                this.nodesMap[edge.target].inEdges.push(edge.id);
            }
        };
        GGraph.prototype.getEdge = function (id) {
            var edgeInternal = this.edgesMap[id];
            return edgeInternal == null ? null : edgeInternal.edge;
        };
        GGraph.prototype.getJSON = function () {
            var igraph = { nodes: this.nodes, edges: this.edges, boundingBox: this.boundingBox, settings: this.settings };
            var ret = JSON.stringify(igraph);
            return ret;
        };
        GGraph.ofJSON = function (json) {
            var igraph = JSON.parse(json);
            if (igraph.edges === undefined)
                igraph.edges = [];
            var ret = new GGraph();
            ret.boundingBox = new GRect(igraph.boundingBox === undefined ? GRect.zero : igraph.boundingBox);
            ret.settings = new GSettings(igraph.settings === undefined ? {} : igraph.settings);
            for (var i = 0; i < igraph.nodes.length; i++) {
                var inode = igraph.nodes[i];
                if (inode.children !== undefined) {
                    var gcluster = new GCluster(inode);
                    ret.addNode(gcluster);
                }
                else {
                    var gnode = new GNode(inode);
                    ret.addNode(gnode);
                }
            }
            for (var i = 0; i < igraph.edges.length; i++) {
                var iedge = igraph.edges[i];
                var gedge = new GEdge(iedge);
                ret.addEdge(gedge);
            }
            return ret;
        };
        GGraph.prototype.createNodeBoundariesRec = function (node, sizer) {
            var cluster = node;
            if (node.boundaryCurve == null) {
                if (node.label != null && node.label.bounds == GRect.zero && sizer !== undefined) {
                    var labelSize = sizer(node.label, node);
                    node.label.bounds = new GRect({ x: 0, y: 0, width: labelSize.x, height: labelSize.y });
                }
                if (cluster.children == null) {
                    var labelWidth = node.label == null ? 0 : node.label.bounds.width;
                    var labelHeight = node.label == null ? 0 : node.label.bounds.height;
                    labelWidth += 2 * node.labelMargin;
                    labelHeight += 2 * node.labelMargin;
                    var boundary;
                    if (node.shape != null && node.shape.shape == GShape.RectShape) {
                        var radiusX = node.shape.radiusX;
                        var radiusY = node.shape.radiusY;
                        if (radiusX == null && radiusY == null) {
                            var k = Math.min(labelWidth, labelHeight);
                            radiusX = radiusY = k / 2;
                        }
                        boundary = new GRoundedRect({
                            bounds: new GRect({ x: 0, y: 0, width: labelWidth, height: labelHeight }), radiusX: radiusX, radiusY: radiusY
                        });
                    }
                    else
                        boundary = GEllipse.make(labelWidth * Math.sqrt(2), labelHeight * Math.sqrt(2));
                    node.boundaryCurve = boundary;
                }
            }
            if (cluster.children != null)
                for (var i = 0; i < cluster.children.length; i++)
                    this.createNodeBoundariesRec(cluster.children[i], sizer);
        };
        GGraph.prototype.createNodeBoundaries = function (sizer) {
            for (var i = 0; i < this.nodes.length; i++)
                this.createNodeBoundariesRec(this.nodes[i], sizer);
            if (sizer !== undefined) {
                for (var i = 0; i < this.edges.length; i++) {
                    var edge = this.edges[i];
                    if (edge.label != null && edge.label.bounds == GRect.zero) {
                        var labelSize = sizer(edge.label, edge);
                        edge.label.bounds = new GRect({ width: labelSize.x, height: labelSize.y });
                    }
                }
            }
        };
        GGraph.contextSizer = function (context, label) {
            return { x: context.measureText(label.content).width, y: parseInt(context.font) };
        };
        GGraph.prototype.createNodeBoundariesFromContext = function (context) {
            var selfMadeContext = (context === undefined);
            if (selfMadeContext) {
                var canvas = document.createElement('canvas');
                document.body.appendChild(canvas);
                context = canvas.getContext("2d");
            }
            this.createNodeBoundaries(function (label) { return GGraph.contextSizer(context, label); });
            if (selfMadeContext)
                document.body.removeChild(canvas);
        };
        GGraph.divSizer = function (div, label) {
            div.innerText = label.content;
            return { x: div.clientWidth, y: div.clientHeight };
        };
        GGraph.prototype.createNodeBoundariesFromDiv = function (div) {
            var selfMadeDiv = (div === undefined);
            if (selfMadeDiv) {
                div = document.createElement('div');
                div.setAttribute('style', 'float:left');
                document.body.appendChild(div);
            }
            this.createNodeBoundaries(function (label) { return GGraph.divSizer(div, label); });
            if (selfMadeDiv)
                document.body.removeChild(div);
        };
        GGraph.SVGSizer = function (svg, label) {
            var element = document.createElementNS('http://www.w3.org/2000/svg', 'text');
            element.setAttribute('fill', 'black');
            var textNode = document.createTextNode(label.content);
            element.appendChild(textNode);
            svg.appendChild(element);
            var bbox = element.getBBox();
            var ret = { x: bbox.width, y: bbox.height };
            svg.removeChild(element);
            if (ret.y > 6)
                ret.y -= 6;
            if (label.content.length == 1)
                ret.x = ret.y;
            return ret;
        };
        GGraph.prototype.createNodeBoundariesFromSVG = function (svg, style) {
            var selfMadeSvg = (svg === undefined);
            if (selfMadeSvg) {
                svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
                if (style !== undefined) {
                    svg.style.font = style.font;
                    svg.style.fontFamily = style.fontFamily;
                    svg.style.fontFeatureSettings = style.fontFeatureSettings;
                    svg.style.fontSize = style.fontSize;
                    svg.style.fontSizeAdjust = style.fontSizeAdjust;
                    svg.style.fontStretch = style.fontStretch;
                    svg.style.fontStyle = style.fontStyle;
                    svg.style.fontVariant = style.fontVariant;
                    svg.style.fontWeight = style.fontWeight;
                }
                document.body.appendChild(svg);
            }
            this.createNodeBoundaries(function (label) { return GGraph.SVGSizer(svg, label); });
            if (selfMadeSvg)
                document.body.removeChild(svg);
        };
        GGraph.prototype.createNodeBoundariesForSVGInContainer = function (container) {
            var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
            container.appendChild(svg);
            this.createNodeBoundaries(function (label) { return GGraph.SVGSizer(svg, label); });
            container.removeChild(svg);
        };
        GGraph.prototype.createNodeBoundariesForSVGWithStyle = function (style) {
            var svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
            if (style != null) {
                svg.style.font = style.font;
                svg.style.fontFamily = style.fontFamily;
                svg.style.fontFeatureSettings = style.fontFeatureSettings;
                svg.style.fontSize = style.fontSize;
                svg.style.fontSizeAdjust = style.fontSizeAdjust;
                svg.style.fontStretch = style.fontStretch;
                svg.style.fontStyle = style.fontStyle;
                svg.style.fontVariant = style.fontVariant;
                svg.style.fontWeight = style.fontWeight;
            }
            document.body.appendChild(svg);
            this.createNodeBoundaries(function (label, owner) { return GGraph.SVGSizer(svg, label); });
            document.body.removeChild(svg);
        };
        GGraph.prototype.stopLayoutGraph = function () {
            if (this.worker != null) {
                this.worker.terminate();
                this.worker = null;
            }
            this.setWorking(false);
        };
        GGraph.prototype.workerCallback = function (msg) {
            var data = msg.data;
            if (data.msgtype == "RunLayout") {
                var runLayoutMsg = data;
                var gs = GGraph.ofJSON(runLayoutMsg.graph);
                this.boundingBox = new GRect({
                    x: gs.boundingBox.x - 10, y: gs.boundingBox.y - 10, width: gs.boundingBox.width + 20, height: gs.boundingBox.height + 20
                });
                for (var nodeId in gs.nodesMap) {
                    var workerNode = gs.nodesMap[nodeId];
                    var myNode = this.getNode(nodeId);
                    myNode.boundaryCurve = workerNode.node.boundaryCurve;
                    if (myNode.label != null)
                        myNode.label.bounds = workerNode.node.label.bounds;
                }
                for (var edgeId in gs.edgesMap) {
                    var workerEdge = gs.edgesMap[edgeId];
                    var myEdge = this.getEdge(edgeId);
                    myEdge.curve = workerEdge.edge.curve;
                    if (myEdge.label != null)
                        myEdge.label.bounds = workerEdge.edge.label.bounds;
                    if (myEdge.arrowHeadAtSource != null)
                        myEdge.arrowHeadAtSource = workerEdge.edge.arrowHeadAtSource;
                    if (myEdge.arrowHeadAtTarget != null)
                        myEdge.arrowHeadAtTarget = workerEdge.edge.arrowHeadAtTarget;
                }
                this.layoutCallbacks.fire();
            }
            else if (data.msgtype == "RouteEdges") {
                var routeEdgesMsg = data;
                var gs = GGraph.ofJSON(routeEdgesMsg.graph);
                for (var edgeId in gs.edgesMap) {
                    var gs = GGraph.ofJSON(routeEdgesMsg.graph);
                    var workerEdge = gs.edgesMap[edgeId];
                    if (routeEdgesMsg.edges == null || routeEdgesMsg.edges.length == 0 || routeEdgesMsg.edges.indexOf(workerEdge.edge.id) >= 0) {
                        var edgeInternal = this.edgesMap[edgeId];
                        var myEdge = edgeInternal.edge;
                        myEdge.curve = workerEdge.edge.curve;
                        edgeInternal.polyline = null;
                        if (myEdge.label != null)
                            myEdge.label.bounds = workerEdge.edge.label.bounds;
                        if (myEdge.arrowHeadAtSource != null)
                            myEdge.arrowHeadAtSource = workerEdge.edge.arrowHeadAtSource;
                        if (myEdge.arrowHeadAtTarget != null)
                            myEdge.arrowHeadAtTarget = workerEdge.edge.arrowHeadAtTarget;
                    }
                }
                this.edgeRoutingCallbacks.fire(routeEdgesMsg.edges);
            }
            else if (data.msgtype == "SetPolyline") {
                var setPolylineMsg = data;
                var edge = this.edgesMap[setPolylineMsg.edge].edge;
                var curve = JSON.parse(setPolylineMsg.curve);
                curve = GCurve.ofCurve(curve);
                edge.curve = curve;
                if (setPolylineMsg.sourceArrowHeadStart != null)
                    edge.arrowHeadAtSource.start = JSON.parse(setPolylineMsg.sourceArrowHeadStart);
                if (setPolylineMsg.sourceArrowHeadEnd != null)
                    edge.arrowHeadAtSource.end = JSON.parse(setPolylineMsg.sourceArrowHeadEnd);
                if (setPolylineMsg.targetArrowHeadStart != null)
                    edge.arrowHeadAtTarget.start = JSON.parse(setPolylineMsg.targetArrowHeadStart);
                if (setPolylineMsg.targetArrowHeadEnd != null)
                    edge.arrowHeadAtTarget.end = JSON.parse(setPolylineMsg.targetArrowHeadEnd);
                this.edgeRoutingCallbacks.fire([edge.id]);
            }
            this.setWorking(false);
        };
        GGraph.prototype.ensureWorkerReady = function () {
            var _this = this;
            if (this.working)
                this.stopLayoutGraph();
            if (this.worker == null) {
                this.worker = new Worker(require.toUrl("./workerBoot.js"));
                var that = this;
                this.worker.addEventListener('message', function (ev) { return _this.workerCallback(ev); });
            }
        };
        GGraph.prototype.setWorking = function (working) {
            this.working = working;
            if (working)
                this.workStartedCallbacks.fire();
            else
                this.workStoppedCallbacks.fire();
        };
        GGraph.prototype.beginLayoutGraph = function () {
            this.ensureWorkerReady();
            this.setWorking(true);
            this.layoutStartedCallbacks.fire();
            var serialisedGraph = this.getJSON();
            var msg = { msgtype: "RunLayout", graph: serialisedGraph };
            this.worker.postMessage(msg);
        };
        GGraph.prototype.beginEdgeRouting = function (edges) {
            this.ensureWorkerReady();
            this.setWorking(true);
            var serialisedGraph = this.getJSON();
            var msg = { msgtype: "RouteEdges", graph: serialisedGraph, edges: edges };
            this.worker.postMessage(msg);
        };
        GGraph.prototype.beginRebuildEdgeCurve = function (edge) {
            this.ensureWorkerReady();
            this.setWorking(true);
            var serialisedGraph = this.getJSON();
            var points = this.edgesMap[edge].polyline;
            var msg = { msgtype: "SetPolyline", graph: serialisedGraph, edge: edge, polyline: JSON.stringify(points) };
            this.worker.postMessage(msg);
        };
        GGraph.prototype.getControlPointAt = function (edge, point) {
            var points = this.getPolyline(edge.id);
            var ret = points[0];
            var dret = ret.dist2(point);
            for (var i = 1; i < points.length; i++) {
                var d = points[i].dist2(point);
                if (d < dret) {
                    ret = points[i];
                    dret = d;
                }
            }
            if (dret > GGraph.EdgeEditCircleRadius * GGraph.EdgeEditCircleRadius)
                ret = null;
            return ret;
        };
        GGraph.prototype.startMoveElement = function (el, mousePoint) {
            if (el instanceof GNode) {
                var node = el;
                var mnt = new MoveNodeToken();
                mnt.node = node;
                mnt.originalBoundsCenter = node.boundaryCurve.getCenter();
                this.moveTokens.push(mnt);
            }
            else if (el instanceof GLabel) {
                var label = el;
                var melt = new MoveEdgeLabelToken();
                melt.label = label;
                melt.originalLabelCenter = label.bounds.getCenter();
                this.moveTokens.push(melt);
            }
            else if (el instanceof GEdge) {
                var edge = el;
                var point = this.getControlPointAt(edge, mousePoint);
                if (point != null) {
                    var met = new MoveEdgeToken();
                    met.edge = edge;
                    met.originalPoint = point;
                    met.originalPolyline = this.getPolyline(edge.id);
                    this.moveTokens.push(met);
                }
            }
        };
        GGraph.prototype.moveElements = function (delta) {
            for (var i in this.moveTokens) {
                var token = this.moveTokens[i];
                if (token instanceof MoveNodeToken) {
                    var ntoken = token;
                    var newBoundaryCenter = ntoken.originalBoundsCenter.add(delta);
                    ntoken.node.boundaryCurve.setCenter(newBoundaryCenter);
                    if (ntoken.node.label != null) {
                        var newLabelCenter = ntoken.originalBoundsCenter.add(delta);
                        ntoken.node.label.bounds.setCenter(newLabelCenter);
                    }
                    this.checkRouteEdges();
                }
                else if (token instanceof MoveEdgeLabelToken) {
                    var ltoken = token;
                    var newBoundsCenter = ltoken.originalLabelCenter.add(delta);
                    ltoken.label.bounds.setCenter(newBoundsCenter);
                }
                else if (token instanceof MoveEdgeToken) {
                    var etoken = token;
                    var newPoint = etoken.originalPoint.add(delta);
                    for (var j = 0; j < etoken.originalPolyline.length; j++)
                        if (etoken.originalPolyline[j].equals(etoken.originalPoint)) {
                            var edgeInternal = this.edgesMap[etoken.edge.id];
                            edgeInternal.polyline = etoken.originalPolyline.map(function (p, k) { return k == j ? newPoint : p; });
                            this.checkRebuildEdge(etoken.edge.id);
                            break;
                        }
                }
            }
        };
        GGraph.prototype.checkRouteEdges = function (edgeSet) {
            var edges = edgeSet == null ? this.getOutdatedEdges() : edgeSet;
            if (edges.length > 0) {
                if (this.delayCheckRouteEdges != null)
                    this.workStoppedCallbacks.remove(this.delayCheckRouteEdges);
                this.delayCheckRouteEdges = null;
                if (this.working) {
                    var that = this;
                    this.delayCheckRouteEdges = function () { that.checkRouteEdges(edges); };
                    this.workStoppedCallbacks.add(this.delayCheckRouteEdges);
                }
                else
                    this.beginEdgeRouting(edges);
            }
        };
        GGraph.prototype.checkRebuildEdge = function (edge) {
            if (this.delayCheckRebuildEdge != null)
                this.workStoppedCallbacks.remove(this.delayCheckRebuildEdge);
            this.delayCheckRebuildEdge = null;
            if (this.working) {
                var that = this;
                this.delayCheckRebuildEdge = function () { that.checkRebuildEdge(edge); };
                this.workStoppedCallbacks.add(this.delayCheckRebuildEdge);
            }
            else
                this.beginRebuildEdgeCurve(edge);
        };
        GGraph.prototype.getOutdatedEdges = function () {
            var affectedEdges = {};
            for (var t in this.moveTokens) {
                var token = this.moveTokens[t];
                if (token instanceof MoveNodeToken) {
                    var ntoken = token;
                    var nEdges = this.getInEdges(ntoken.node.id).concat(this.getOutEdges(ntoken.node.id)).concat(this.getSelfEdges(ntoken.node.id));
                    for (var edge in nEdges)
                        affectedEdges[nEdges[edge]] = true;
                }
            }
            var edges = [];
            for (var e in affectedEdges)
                edges.push(e);
            return edges;
        };
        GGraph.prototype.endMoveElements = function () {
            this.checkRouteEdges();
            this.moveTokens = [];
        };
        GGraph.prototype.removeColinearVertices = function (polyline) {
            for (var i = 1; i < polyline.length - 2; i++) {
                var a = GPoint.signedDoubledTriangleArea(polyline[i - 1], polyline[i], polyline[i + 1]);
                if (a >= -GGraph.ColinearityEpsilon && a <= GGraph.ColinearityEpsilon)
                    polyline.splice(i--, 1);
            }
        };
        GGraph.prototype.makePolyline = function (edge) {
            var points = [];
            var source = this.nodesMap[edge.source];
            points.push(source.node.boundaryCurve.getCenter());
            if (edge.curve != null && edge.curve.curvetype == "SegmentedCurve") {
                var scurve = edge.curve;
                points.push(scurve.getStart());
                for (var i = 0; i < scurve.segments.length; i++)
                    points.push(scurve.segments[i].getEnd());
            }
            var target = this.nodesMap[edge.target];
            points.push(target.node.boundaryCurve.getCenter());
            this.removeColinearVertices(points);
            return points;
        };
        GGraph.prototype.getPolyline = function (edgeID) {
            var edgeInternal = this.edgesMap[edgeID];
            if (edgeInternal.polyline == null)
                edgeInternal.polyline = this.makePolyline(edgeInternal.edge);
            return edgeInternal.polyline;
        };
        GGraph.prototype.addEdgeControlPoint = function (edgeID, point) {
            var edgeInternal = this.edgesMap[edgeID];
            var iclosest = 0;
            var dclosest = edgeInternal.polyline[0].dist2(point);
            for (var i = 0; i < edgeInternal.polyline.length; i++) {
                var d = edgeInternal.polyline[i].dist2(point);
                if (d < dclosest) {
                    iclosest = i;
                    dclosest = d;
                }
            }
            if (iclosest == edgeInternal.polyline.length - 1)
                iclosest--;
            else if (iclosest > 0) {
                var par = point.closestParameter(edgeInternal.polyline[iclosest - 1], edgeInternal.polyline[iclosest]);
                if (par > 0.1 && par < 0.9)
                    iclosest--;
            }
            edgeInternal.polyline.splice(iclosest + 1, 0, point);
            this.beginRebuildEdgeCurve(edgeID);
        };
        GGraph.prototype.delEdgeControlPoint = function (edgeID, point) {
            var edgeInternal = this.edgesMap[edgeID];
            for (var i = 1; i < edgeInternal.polyline.length - 1; i++)
                if (edgeInternal.polyline[i].equals(point)) {
                    edgeInternal.polyline.splice(i, 1);
                    this.beginRebuildEdgeCurve(edgeID);
                    break;
                }
        };
        GGraph.EdgeEditCircleRadius = 8;
        GGraph.ColinearityEpsilon = 50.00;
        return GGraph;
    }());
    exports.GGraph = GGraph;
});
//# sourceMappingURL=ggraph.js.map