define(["require", "exports", "./ggraph"], function (require, exports, G) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.handleMessage = void 0;
    importScripts("sharpkit_pre.js");
    importScripts("jsclr.js");
    importScripts("Microsoft.Msagl.js");
    importScripts("sharpkit_post.js");
    var LayoutWorker = (function () {
        function LayoutWorker(graph) {
            this.originalGraph = graph;
        }
        LayoutWorker.prototype.getMsaglPoint = function (ipoint) {
            return new Microsoft.Msagl.Core.Geometry.Point.ctor$$Double$$Double(ipoint.x, ipoint.y);
        };
        LayoutWorker.prototype.getMsaglRect = function (grect) {
            return new Microsoft.Msagl.Core.Geometry.Rectangle.ctor$$Double$$Double$$Point(grect.x, grect.y, this.getMsaglPoint({ x: grect.width, y: grect.height }));
        };
        LayoutWorker.prototype.getMsaglCurve = function (gcurve) {
            if (gcurve == null)
                return null;
            else if (gcurve.curvetype == "Ellipse") {
                var gellipse = gcurve;
                return new Microsoft.Msagl.Core.Geometry.Curves.Ellipse.ctor$$Double$$Double$$Point$$Point$$Point(gellipse.parStart, gellipse.parEnd, this.getMsaglPoint(gellipse.axisA), this.getMsaglPoint(gellipse.axisB), this.getMsaglPoint(gellipse.center));
            }
            else if (gcurve.curvetype == "Line") {
                var gline = gcurve;
                return new Microsoft.Msagl.Core.Geometry.Curves.LineSegment.ctor$$Point$$Point(this.getMsaglPoint(gline.start), this.getMsaglPoint(gline.end));
            }
            else if (gcurve.curvetype == "Bezier") {
                var gbezier = gcurve;
                return new Microsoft.Msagl.Core.Geometry.Curves.CubicBezierSegment.ctor(this.getMsaglPoint(gbezier.start), this.getMsaglPoint(gbezier.p1), this.getMsaglPoint(gbezier.p2), this.getMsaglPoint(gbezier.p3));
            }
            else if (gcurve.curvetype == "Polyline") {
                var gpolyline = gcurve;
                var points = [];
                for (var i = 0; i < gpolyline.points.length; i++)
                    points.push(this.getMsaglPoint(gpolyline.points[i]));
                return new Microsoft.Msagl.Core.Geometry.Curves.Polyline.ctor$$IEnumerable$1$Point(points);
            }
            else if (gcurve.curvetype == "RoundedRect") {
                var groundedrect = gcurve;
                return new Microsoft.Msagl.Core.Geometry.Curves.RoundedRect.ctor$$Rectangle$$Double$$Double(this.getMsaglRect(groundedrect.bounds), groundedrect.radiusX, groundedrect.radiusY);
            }
            else if (gcurve.curvetype == "SegmentedCurve") {
                var gsegcurve = gcurve;
                var curves = [];
                for (var i = 0; i < gsegcurve.segments.length; i++)
                    curves.push(this.getMsaglCurve(gsegcurve.segments[i]));
                return new Microsoft.Msagl.Core.Geometry.Curves.Curve.ctor$$List$1$ICurve(curves);
            }
            return null;
        };
        LayoutWorker.prototype.addNodeToMsagl = function (graph, rootCluster, nodeMap, gnode) {
            var children = gnode.children;
            var node = null;
            if (children != null) {
                var gcluster = gnode;
                node = new Microsoft.Msagl.Core.Layout.Cluster.ctor();
                for (var _i = 0, children_1 = children; _i < children_1.length; _i++) {
                    var child = children_1[_i];
                    var childNode = this.addNodeToMsagl(graph, node, nodeMap, child);
                    if (child.children == null)
                        node.AddNode(childNode);
                }
                node.set_GeometryParent(rootCluster);
                var rectangularBoundary = new Microsoft.Msagl.Core.Geometry.RectangularClusterBoundary.ctor();
                rectangularBoundary.set_BottomMargin(gcluster.margin.bottom);
                rectangularBoundary.set_TopMargin(gcluster.margin.top);
                rectangularBoundary.set_RightMargin(gcluster.margin.right);
                rectangularBoundary.set_LeftMargin(gcluster.margin.left);
                rectangularBoundary.set_MinWidth(gcluster.margin.minWidth);
                rectangularBoundary.set_MinHeight(gcluster.margin.minHeight);
                node.set_RectangularBoundary(rectangularBoundary);
                rootCluster.AddCluster(node);
            }
            else {
                node = new Microsoft.Msagl.Core.Layout.Node.ctor();
                graph.get_Nodes().Add(node);
                var curve = this.getMsaglCurve(gnode.boundaryCurve);
                if (curve == null)
                    curve = this.getMsaglCurve(new G.GRoundedRect({
                        bounds: { x: 0, y: 0, width: 0, height: 0 }, radiusX: 0, radiusY: 0
                    }));
                node.set_BoundaryCurve(curve);
            }
            node.set_UserData(gnode.id);
            nodeMap[gnode.id] = { mnode: node, gnode: gnode };
            return node;
        };
        LayoutWorker.prototype.addEdgeToMsagl = function (graph, nodeMap, edgeMap, gedge) {
            var source = nodeMap[gedge.source].mnode;
            var target = nodeMap[gedge.target].mnode;
            var edge = new Microsoft.Msagl.Core.Layout.Edge.ctor$$Node$$Node(source, target);
            var curve = this.getMsaglCurve(gedge.curve);
            if (curve != null)
                edge.set_Curve(curve);
            if (gedge.label != null) {
                var label = new Microsoft.Msagl.Core.Layout.Label.ctor$$Double$$Double$$GeometryObject(gedge.label.bounds.width, gedge.label.bounds.height, edge);
                label.set_GeometryParent(edge);
                edge.set_Label(label);
            }
            if (gedge.arrowHeadAtSource != null)
                edge.get_EdgeGeometry().set_SourceArrowhead(new Microsoft.Msagl.Core.Layout.Arrowhead.ctor());
            if (gedge.arrowHeadAtTarget != null)
                edge.get_EdgeGeometry().set_TargetArrowhead(new Microsoft.Msagl.Core.Layout.Arrowhead.ctor());
            graph.get_Edges().Add(edge);
            edgeMap[gedge.id] = { medge: edge, gedge: gedge };
        };
        LayoutWorker.prototype.getMsagl = function (ggraph) {
            var nodeMap = {};
            var edgeMap = {};
            var graph = new Microsoft.Msagl.Core.Layout.GeometryGraph.ctor();
            var rootCluster = graph.get_RootCluster();
            rootCluster.set_GeometryParent(graph);
            for (var i = 0; i < ggraph.nodes.length; i++)
                this.addNodeToMsagl(graph, rootCluster, nodeMap, ggraph.nodes[i]);
            for (var i = 0; i < ggraph.edges.length; i++)
                this.addEdgeToMsagl(graph, nodeMap, edgeMap, ggraph.edges[i]);
            var settings;
            if (ggraph.settings.layout == G.GSettings.mdsLayout) {
                settings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings.ctor();
                settings.set_IterationsWithMajorization(settings.iterationsWithMajorization);
            }
            else {
                settings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings.ctor();
                var transformation = new Microsoft.Msagl.Core.Geometry.Curves.PlaneTransformation.ctor$$Double$$Double$$Double$$Double$$Double$$Double(ggraph.settings.transformation.m00, ggraph.settings.transformation.m01, ggraph.settings.transformation.m02, ggraph.settings.transformation.m10, ggraph.settings.transformation.m11, ggraph.settings.transformation.m12);
                settings.set_Transformation(transformation);
                settings.set_AspectRatio(ggraph.settings.aspectRatio);
                for (var i = 0; i < ggraph.settings.upDownConstraints.length; i++) {
                    var upNode = nodeMap[ggraph.settings.upDownConstraints[i].upNode].mnode;
                    var downNode = nodeMap[ggraph.settings.upDownConstraints[i].downNode].mnode;
                    settings.AddUpDownConstraint(upNode, downNode);
                }
            }
            var edgeRoutingSettings = settings.get_EdgeRoutingSettings();
            if (ggraph.settings.routing == G.GSettings.splinesRouting)
                edgeRoutingSettings.set_EdgeRoutingMode(Microsoft.Msagl.Core.Routing.EdgeRoutingMode.Spline);
            else if (ggraph.settings.routing == G.GSettings.splinesBundlingRouting)
                edgeRoutingSettings.set_EdgeRoutingMode(Microsoft.Msagl.Core.Routing.EdgeRoutingMode.SplineBundling);
            else if (ggraph.settings.routing == G.GSettings.straightLineRouting)
                edgeRoutingSettings.set_EdgeRoutingMode(Microsoft.Msagl.Core.Routing.EdgeRoutingMode.StraightLine);
            else if (ggraph.settings.routing == G.GSettings.rectilinearRouting)
                edgeRoutingSettings.set_EdgeRoutingMode(Microsoft.Msagl.Core.Routing.EdgeRoutingMode.Rectilinear);
            else if (ggraph.settings.routing == G.GSettings.rectilinearToCenterRouting)
                edgeRoutingSettings.set_EdgeRoutingMode(Microsoft.Msagl.Core.Routing.EdgeRoutingMode.RectilinearToCenter);
            return { graph: graph, settings: settings, nodeMap: nodeMap, edgeMap: edgeMap, source: ggraph };
        };
        LayoutWorker.prototype.getGPoint = function (point) {
            return new G.GPoint({ x: point.get_X(), y: point.get_Y() });
        };
        LayoutWorker.prototype.getGRect = function (rect) {
            return new G.GRect({ x: rect.get_Left(), y: rect.get_Bottom(), width: rect.get_Width(), height: rect.get_Height() });
        };
        LayoutWorker.prototype.getGCurve = function (curve) {
            var ret;
            if (Is(curve, Microsoft.Msagl.Core.Geometry.Curves.Curve.ctor)) {
                var segments = [];
                var sEn = curve.get_Segments().GetEnumerator();
                while (sEn.MoveNext())
                    segments.push(this.getGCurve(sEn.get_Current()));
                ret = new G.GSegmentedCurve({
                    type: "SegmentedCurve",
                    segments: segments
                });
            }
            else if (Is(curve, Microsoft.Msagl.Core.Geometry.Curves.Polyline.ctor)) {
                var points = [];
                var pEn = curve.get_PolylinePoints().GetEnumerator();
                while (pEn.MoveNext())
                    points.push(this.getGPoint(pEn.get_Current()));
                ret = new G.GPolyline({
                    type: "Polyline",
                    start: this.getGPoint(curve.get_Start()),
                    points: points,
                    closed: curve.get_Closed()
                });
            }
            else if (Is(curve, Microsoft.Msagl.Core.Geometry.Curves.CubicBezierSegment.ctor)) {
                ret = new G.GBezier({
                    type: "Bezier",
                    start: this.getGPoint(curve.get_Start()),
                    p1: this.getGPoint(curve.B(1)),
                    p2: this.getGPoint(curve.B(2)),
                    p3: this.getGPoint(curve.B(3)),
                });
            }
            else if (Is(curve, Microsoft.Msagl.Core.Geometry.Curves.LineSegment.ctor)) {
                ret = new G.GLine({
                    type: "Line",
                    start: this.getGPoint(curve.get_Start()),
                    end: this.getGPoint(curve.get_End())
                });
            }
            else if (Is(curve, Microsoft.Msagl.Core.Geometry.Curves.Ellipse.ctor)) {
                ret = new G.GEllipse({
                    type: "Ellipse",
                    axisA: this.getGPoint(curve.get_AxisA()),
                    axisB: this.getGPoint(curve.get_AxisB()),
                    center: this.getGPoint(curve.get_Center()),
                    parEnd: curve.get_ParEnd(),
                    parStart: curve.get_ParStart()
                });
            }
            else if (Is(curve, Microsoft.Msagl.Core.Geometry.Curves.RoundedRect.ctor)) {
                ret = new G.GRoundedRect({
                    type: "RoundedRect",
                    bounds: this.getGRect(curve.get_BoundingBox()),
                    radiusX: curve.get_RadiusX(),
                    radiusY: curve.get_RadiusY()
                });
            }
            return ret;
        };
        LayoutWorker.prototype.getGGraph = function (msagl) {
            msagl.source.boundingBox = this.getGRect(msagl.graph.get_BoundingBox());
            for (var id in msagl.nodeMap) {
                var node = msagl.nodeMap[id].mnode;
                var gnode = msagl.nodeMap[id].gnode;
                var curve = node.get_BoundaryCurve();
                gnode.boundaryCurve = this.getGCurve(curve);
                if (gnode.label != null) {
                    if (gnode.children != null) {
                        gnode.label.bounds.x = node.get_Center().get_X() - gnode.label.bounds.width / 2;
                        gnode.label.bounds.y = node.get_BoundingBox().get_Bottom() + gnode.label.bounds.height / 2;
                    }
                    else {
                        gnode.label.bounds.x = node.get_Center().get_X() - gnode.label.bounds.width / 2;
                        gnode.label.bounds.y = node.get_Center().get_Y() - gnode.label.bounds.height / 2;
                    }
                }
            }
            for (var id in msagl.edgeMap) {
                var edge = msagl.edgeMap[id].medge;
                var gedge = msagl.edgeMap[id].gedge;
                var curve = edge.get_Curve();
                if (curve == null)
                    console.log("MSAGL warning: layout engine did not create a curve for the edge " + id);
                else {
                    if (gedge.label != null) {
                        var labelbbox = this.getGRect(edge.get_Label().get_BoundingBox());
                        gedge.label.bounds.x = labelbbox.x;
                        gedge.label.bounds.y = labelbbox.y;
                    }
                    if (gedge.arrowHeadAtSource != null) {
                        gedge.arrowHeadAtSource.start = this.getGPoint(curve.get_Start());
                        gedge.arrowHeadAtSource.end = this.getGPoint(edge.get_EdgeGeometry().get_SourceArrowhead().get_TipPosition());
                    }
                    if (gedge.arrowHeadAtTarget != null) {
                        gedge.arrowHeadAtTarget.start = this.getGPoint(curve.get_End());
                        gedge.arrowHeadAtTarget.end = this.getGPoint(edge.get_EdgeGeometry().get_TargetArrowhead().get_TipPosition());
                    }
                    gedge.curve = this.getGCurve(curve);
                    if (gedge.curve == null)
                        console.log("MSAGL warning: failed to translate curve for the edge " + id);
                }
            }
            return msagl.source;
        };
        LayoutWorker.prototype.runLayout = function () {
            var msagl = this.getMsagl(this.originalGraph);
            var cancelToken = new Microsoft.Msagl.Core.CancelToken.ctor();
            cancelToken.set_Canceled(false);
            Microsoft.Msagl.Miscellaneous.LayoutHelpers.CalculateLayout(msagl.graph, msagl.settings, cancelToken);
            this.finalGraph = this.getGGraph(msagl);
        };
        LayoutWorker.prototype.runEdgeRouting = function (edgeIDs) {
            if (this.originalGraph.settings.routing == G.GSettings.sugiyamaSplinesRouting)
                this.originalGraph.settings.routing = G.GSettings.splinesRouting;
            var msagl = this.getMsagl(this.originalGraph);
            var edges = [];
            if (edgeIDs == null || edgeIDs.length == 0)
                for (var id in msagl.edgeMap)
                    edges.push(msagl.edgeMap[id].medge);
            else
                for (var i = 0; i < edgeIDs.length; i++) {
                    var msaglEdge = msagl.edgeMap[edgeIDs[i]].medge;
                    edges.push(msaglEdge);
                }
            var router = new Microsoft.Msagl.Routing.SplineRouter.ctor$$GeometryGraph$$Double$$Double$$Double$$BundlingSettings(msagl.graph, msagl.settings.get_EdgeRoutingSettings().get_Padding(), msagl.settings.get_EdgeRoutingSettings().get_PolylinePadding(), msagl.settings.get_EdgeRoutingSettings().get_ConeAngle(), msagl.settings.get_EdgeRoutingSettings().get_BundlingSettings());
            router.Run();
            var elp = new Microsoft.Msagl.Core.Layout.EdgeLabelPlacement.ctor$$GeometryGraph(msagl.graph);
            elp.Run();
            this.finalGraph = this.getGGraph(msagl);
        };
        LayoutWorker.prototype.setPolyline = function (edge, points) {
            var msagl = this.getMsagl(this.originalGraph);
            var medge = msagl.edgeMap[edge].medge;
            var mpoints = points.map(this.getMsaglPoint);
            var mpolyline = Microsoft.Msagl.Core.Geometry.SmoothedPolyline.FromPoints(mpoints);
            var mcurve = mpolyline.CreateCurve();
            if (!Microsoft.Msagl.Core.Layout.Arrowheads.TrimSplineAndCalculateArrowheads$$Edge$$ICurve$$Boolean$$Boolean(medge, mcurve, true, false))
                Microsoft.Msagl.Core.Layout.Arrowheads.CreateBigEnoughSpline(medge);
            mcurve = medge.get_Curve();
            var curve = this.getGCurve(mcurve);
            var hasSourceArrowhead = medge.get_EdgeGeometry().get_SourceArrowhead() != null;
            var hasTargetArrowhead = medge.get_EdgeGeometry().get_TargetArrowhead() != null;
            var sourceArrowHeadStart = hasSourceArrowhead ? this.getGPoint(mcurve.get_Start()) : null;
            var sourceArrowHeadEnd = hasSourceArrowhead ? this.getGPoint(medge.get_EdgeGeometry().get_SourceArrowhead().get_TipPosition()) : null;
            var targetArrowHeadStart = hasTargetArrowhead ? this.getGPoint(mcurve.get_End()) : null;
            var targetArrowHeadEnd = hasTargetArrowhead ? this.getGPoint(medge.get_EdgeGeometry().get_TargetArrowhead().get_TipPosition()) : null;
            return { curve: curve, sourceArrowHeadStart: sourceArrowHeadStart, sourceArrowHeadEnd: sourceArrowHeadEnd, targetArrowHeadStart: targetArrowHeadStart, targetArrowHeadEnd: targetArrowHeadEnd };
        };
        return LayoutWorker;
    }());
    function handleMessage(e) {
        var message = e.data;
        var ggraph = G.GGraph.ofJSON(message.graph);
        var worker = new LayoutWorker(ggraph);
        var answer = null;
        switch (message.msgtype) {
            case "RunLayout":
                {
                    try {
                        worker.runLayout();
                        answer = { msgtype: "RunLayout", graph: worker.finalGraph.getJSON() };
                    }
                    catch (e) {
                        if (e.message != null)
                            e = e.message;
                        console.log("error in MSAGL.RunLayout: " + JSON.stringify(e));
                        answer = { msgtype: "Error", error: e };
                    }
                    break;
                }
            case "RouteEdges":
                {
                    var edges = message.edges;
                    try {
                        worker.runEdgeRouting(edges);
                        answer = { msgtype: "RouteEdges", graph: worker.finalGraph.getJSON(), edges: edges };
                    }
                    catch (e) {
                        if (e.message != null)
                            e = e.message;
                        console.log("error in MSAGL.RouteEdges: " + JSON.stringify(e));
                        answer = { msgtype: "Error", error: e };
                    }
                    break;
                }
            case "SetPolyline":
                {
                    var edge = message.edge;
                    var points = JSON.parse(message.polyline);
                    var result = null;
                    try {
                        result = worker.setPolyline(edge, points);
                        answer = {
                            msgtype: "SetPolyline", edge: edge, curve: JSON.stringify(result.curve),
                            sourceArrowHeadStart: result.sourceArrowHeadStart == null ? null : JSON.stringify(result.sourceArrowHeadStart),
                            sourceArrowHeadEnd: result.sourceArrowHeadEnd == null ? null : JSON.stringify(result.sourceArrowHeadEnd),
                            targetArrowHeadStart: result.targetArrowHeadStart == null ? null : JSON.stringify(result.targetArrowHeadStart),
                            targetArrowHeadEnd: result.targetArrowHeadEnd == null ? null : JSON.stringify(result.targetArrowHeadEnd)
                        };
                    }
                    catch (e) {
                        if (e.message != null)
                            e = e.message;
                        console.log("error in MSAGL.SetPolyline: " + JSON.stringify(e));
                        answer = { msgtype: "Error", error: e };
                    }
                    break;
                }
        }
        self.postMessage(answer);
    }
    exports.handleMessage = handleMessage;
    self.addEventListener('message', handleMessage);
});
//# sourceMappingURL=msagl.worker.js.map