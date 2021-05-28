using Data;

namespace Script
{
    public class PMServer : Server, IGameScripts
    {
        public override BaseData baseData { get { return this.pmData; } }
        
        public PMData pmData;
        public PMScript pmScript;
        public PMSqlUtils pmSqlUtils;
        public PMPlayerToSqlTablePlayer pmPlayerToSqlTablePlayer;
        public PMScriptCreateNewPlayer pmScriptCreateNewPlayer;

        public SCUtils scUtils { get; set; }
        public GameScript gameScript;
    }
}