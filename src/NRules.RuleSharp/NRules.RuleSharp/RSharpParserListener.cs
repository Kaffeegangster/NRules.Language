﻿using System.Linq;
using Antlr4.Runtime.Tree;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.RuleSharp
{
    internal class RuleSharpParserListener : RuleSharpParserBaseListener
    {
        private readonly RuleParserContext _parserContext;
        private readonly RuleSet _ruleSet;

        public RuleSharpParserListener(RuleParserContext parserContext, RuleSet ruleSet)
        {
            _parserContext = parserContext;
            _ruleSet = ruleSet;
        }

        public override void EnterUsingNamespaceDirective(RuleSharpParser.UsingNamespaceDirectiveContext context)
        {
            var @namespace = context.namespace_or_type_name().GetText();
            _parserContext.Loader.AddNamespace(@namespace);
        }

        public override void EnterUsingAliasDirective(RuleSharpParser.UsingAliasDirectiveContext context)
        {
            var alias = context.identifier().GetText();
            var typeName = context.namespace_or_type_name().GetText();
            _parserContext.Loader.AddAlias(alias, typeName);
        }

        public override void EnterRule_definition(RuleSharpParser.Rule_definitionContext context)
        {
            var builder = new RuleBuilder();

            var expressionListener = new RuleParserListener(_parserContext, builder);
            var walker = new ParseTreeWalker();
            walker.Walk(expressionListener, context);

            var rule = builder.Build();
            _ruleSet.Add(Enumerable.Repeat(rule, 1));
        }
    }
}