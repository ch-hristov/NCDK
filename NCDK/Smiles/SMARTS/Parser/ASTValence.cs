/* Generated By:JJTree: Do not edit this line. ASTValence.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace NCDK.Smiles.SMARTS.Parser
{

    public
    class ASTValence : SimpleNode
    {
        public ASTValence(int id)
          : base(id)
        {
        }

        public ASTValence(SMARTSParser p, int id)
          : base(p, id)
        {
        }

        /// <summary>
        /// valence order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>Accept the visitor. </summary>
        public override object JJTAccept(SMARTSParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}