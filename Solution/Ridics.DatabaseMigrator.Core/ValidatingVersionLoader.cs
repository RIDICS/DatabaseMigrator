using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;

namespace Ridics.DatabaseMigrator.Core
{
    public class ValidatingVersionLoader : VersionLoader
    {
        private readonly IMigrationProcessor m_processor;
        public IDictionary<long, StoredMigrationInfo> StoredMigrationInfos { get; private set; }

        public ValidatingVersionLoader(
            IProcessorAccessor processorAccessor, IConventionSet conventionSet, IMigrationRunnerConventions conventions,
            IVersionTableMetaData versionTableMetaData, IMigrationRunner runner
        ) : base(processorAccessor, conventionSet, conventions, versionTableMetaData, runner)
        {
            m_processor = processorAccessor.Processor;

            LoadVersionInfo();
        }

        public new void LoadVersionInfo()
        {
            base.LoadVersionInfo();

            if (StoredMigrationInfos != null)
            {
                return;
            }

            var dataSet = m_processor.ReadTableData(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName);

            var storedMigrationInfos = new List<StoredMigrationInfo>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                storedMigrationInfos.Add(
                    new StoredMigrationInfo(
                        (long) row[VersionTableMetaData.ColumnName],
                        (string) row[VersionTableMetaData.DescriptionColumnName],
                        (DateTime) row[VersionTableMetaData.AppliedOnColumnName]
                    )
                );
            }

            storedMigrationInfos.Sort((a, b) => a.Version.CompareTo(b.Version));

            StoredMigrationInfos = storedMigrationInfos.ToDictionary(x => x.Version);
        }
    }
}
