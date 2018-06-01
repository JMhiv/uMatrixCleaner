﻿using System.Collections.Generic;
using System.Linq;
using uMatrixCleaner;
using Xunit;

namespace Tests
{
    public class TestMain
    {
        [Fact]
        public void TestDeduplicate()
        {
            var r1 = new UMatrixRule("* 1st-party css block");
            var r2 = new UMatrixRule("google.com * css allow");
            var r3 = new UMatrixRule("mail.google.com * css allow");

            var rules = new LinkedList<UMatrixRule>(new[] { r1, r2, r3 });
            Program.Deduplicate(rules, new System.Predicate<UMatrixRule>[0]);

            Assert.Equal(2, rules.Count);
            Assert.Contains(r1, rules);
            Assert.Contains(r2, rules);

            r1 = new UMatrixRule("login.mail.google.com * css allow");
            r2 = new UMatrixRule("google.com * css allow");
            r3 = new UMatrixRule("mail.google.com * css block");
            rules = new LinkedList<UMatrixRule>(new[] { r1, r2, r3 });
            Program.Deduplicate(rules, new System.Predicate<UMatrixRule>[0]);
            Assert.Contains(r1, rules);
            Assert.Contains(r2, rules);
            Assert.Contains(r3, rules);
        }

        [Fact]
        public void TestMergeDestination()
        {
            var input = @"
* * * block
* * css allow
* * frame block
* * image allow
* * script block
* 1st-party * allow
* 1st-party frame allow
* 1st-party script allow";
            var rules = new LinkedList<UMatrixRule>(from line in input.Split("\r\n")
                                                    where line.Length > 0
                                                    select new UMatrixRule(line));
            var r1 = new UMatrixRule("appledaily.com rtnvideo1.appledaily.com.tw media allow");
            var r2 = new UMatrixRule("appledaily.com video.appledaily.com.tw media allow");

            rules.AddLast(r1);
            rules.AddLast(r2);

            Program.Merge(rules, 2);

            Assert.DoesNotContain( r1,rules);
            Assert.DoesNotContain(r2, rules);
            var r3 = new UMatrixRule("appledaily.com appledaily.com.tw media allow");
            Assert.Contains(r3, rules);
        }
    }
}
