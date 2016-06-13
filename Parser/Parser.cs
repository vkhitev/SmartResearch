﻿using java.io;
using edu.stanford.nlp.process;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.trees;
using edu.stanford.nlp.parser.lexparser;
using Console = System.Console;

namespace Parser
{
    public class Parser
    {
        public Parser()
        {
        }

        public void Parse(string text)
        {
            LexicalizedParser lp = LexicalizedParser.loadModel("englishPCFG.ser.gz");

            var jarRoot = @"..\..\..\Parser\stanford-parser-full-2015-12-09\models";
            var modelsDirectory = jarRoot + @"\edu\stanford\nlp\models";

            // Loading english PCFG parser from file
            //var lp = LexicalizedParser.loadModel(modelsDirectory + @"\lexparser\englishPCFG.ser.gz");

            // This sample shows parsing a list of correctly tokenized words
            //var sent = new[] { "This", "is", "an", "easy", "sentence", "." };
            //var rawWords = Sentence.toCoreLabelList(sent);
            //var tree = lp.apply(rawWords);
            //tree.pennPrint();

            // This option shows loading and using an explicit tokenizer
            var sent2 = "This is another sentence.";
            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var sent2Reader = new StringReader(sent2);
            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
            sent2Reader.close();
            var tree2 = lp.apply(rawWords2);

            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(tree2);
            var tdl = gs.typedDependenciesCCprocessed();

            for (int i = 0; i < tdl.size(); i++)
            {
                Console.WriteLine(tdl.get(i));
            }

            // Extract collapsed dependencies from parsed tree
            //var tp = new TreePrint("penn,typedDependenciesCollapsed");
            //tp.printTree(tree2);
        }
    }
}
