/* Generated By:JJTree: Do not edit this line. ASTTotalConnectivity.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace NCDK.Smiles.SMARTS.Parser
{
    internal class ASTTotalConnectivity : SimpleNode
    {
        public ASTTotalConnectivity(int id)
          : base(id)
        {
        }

        public ASTTotalConnectivity(SMARTSParser p, int id)
          : base(p, id)
        {
        }

        /// <summary>
        /// The number of total connections.
        /// </summary>
        public int NumOfConnection { get; set; }

        /// <summary>Accept the visitor. </summary>
        public override object JjtAccept(ISMARTSParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}
