
parser grammar DotParser;

options
{
    importVocab=Dot;
}
graph
    :  (STRICT_LITERAL)?
       (GRAPH_LITERAL | DIGRAPH_LITERAL) (ID)?
       O_BRACKET stmt_list C_BRACKET
    ; stmt_list
    :  ( stmt SEMI_COLON stmt_list)?
    ; stmt
    :  (ID
          (  EQUAL ID
           | node_or_edge_stmt)
        | attr_stmt
        | subgraph
       )
    ; node_or_edge_stmt
    :  (port)?
       ( edge_stmt
        | node_stmt)
    ; edge_stmt
    :  edgeRHS (attr_list)?
    ; node_stmt
    :  (attr_list)?
    ; attr_stmt
    :  (GRAPH_LITERAL | NODE_LITERAL | EDGE_LITERAL) (attr_list)
    ; attr_list
    :  O_SQR_BRACKET (a_list)? C_SQR_BRACKET (attr_list)?
    ; a_list
    :  (arg) (COMMA)? (a_list)?
    ; arg
    :  ID (EQUAL ID)?
    ; edgeRHS
    :  EDGEOP_LITERAL (node_id | subgraph)? (edgeRHS)?
    ; node_id
    :  ID (port)?
    ; port
    :  COLON
        (ID (COLONCOMPASS_PT)?
         |COMPASS_PT
        )
    ; subgraph
    :  (SUBGRAPH_LITERAL
           (
                ( subgraph_ext
                | subgraph_simple)
            | subgraph_ext))
    ; subgraph_ext
    :  (SUBGRAPH_LITERAL (ID)? )? O_BRACKET stmt_list C_BRACKET
    ; subgraph_simple
    :  SUBGRAPH_LITERAL ID
    ;