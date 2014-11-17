namespace NDDDSample.Web.Initializers
{
    using NHibernate.Cfg;
    using NHibernate.Tool.hbm2ddl;
    using Persistence.NHibernate.Utils;
    using Rhino.Commons;

    public static class NhInitializer
    {
        public static void Init()
        {
            Configuration configuration = new Configuration().Configure();
            /*
            new SchemaExport(configuration)
                .Execute(true, true, false, UnitOfWork.CurrentSession.Connection, null);
            SampleDataGenerator.LoadSampleData();
            */
        }
    }
}