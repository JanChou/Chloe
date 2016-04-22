﻿using Chloe.DbExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.Query
{
    public class ResultElement
    {
        public const string DefaultTableAlias = "T";

        public ResultElement()
        {
            this.OrderSegments = new List<DbOrderSegmentExpression>();
        }

        public IMappingObjectExpression MappingObjectExpression { get; set; }

        /// <summary>
        /// 表示当前 OrderParts 集合内的排序是否是从上个 query 继承来的
        /// </summary>
        public bool IsOrderSegmentsFromSubQuery { get; set; }

        public List<DbOrderSegmentExpression> OrderSegments { get; private set; }

        /// <summary>
        /// 如 takequery 了以后，则 table 的 Expression 类似 (select T.Id.. from User as T),Alias 则为新生成的
        /// </summary>
        public DbFromTableExpression FromTable { get; set; }
        public DbExpression Where { get; private set; }

        public void UpdateCondition(DbExpression whereExpression)
        {
            if (this.Where == null)
                this.Where = whereExpression;
            else
                this.Where = new DbAndExpression(this.Where, whereExpression);
        }

        public string GenerateUniqueTableAlias(string prefix = DefaultTableAlias)
        {
            if (this.FromTable == null)
                return prefix;

            string alias = prefix;
            int i = 0;
            DbFromTableExpression fromTable = this.FromTable;
            while (ExistTableAlias(fromTable, alias))
            {
                alias = prefix + i.ToString();
                i++;
            }

            return alias;
        }

        static bool ExistTableAlias(DbFromTableExpression fromTable, string alias)
        {
            if (fromTable.Table.Alias == alias)
                return true;

            foreach (var item in fromTable.JoinTables)
            {
                if (ExistTableAlias(item, alias))
                    return true;
            }

            return false;
        }
        static bool ExistTableAlias(DbJoinTableExpression joinTable, string alias)
        {
            if (joinTable.Table.Alias == alias)
                return true;

            foreach (var item in joinTable.JoinTables)
            {
                if (ExistTableAlias(item, alias))
                    return true;
            }

            return false;
        }
    }
}
