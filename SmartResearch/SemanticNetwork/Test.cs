using System;
using System.Collections.Generic;

using System.Net;

namespace SmartResearch.SemanticNetwork
{
    public class Test
    {
        public void Start()
        {
            List<Process> pr1 = new List<Process> { new Process("Animal","has","Brain"),
                                                    new Process("Animal","live","Life"),
                                                    new Process("Cat","has","Claws"),
                                                    new Process("Cat","has","Fur"),
                                                    new Process("Cat","has","Size"),
                                                    new Process("Fur", "has", "Color"),
                                                    new Process("Cat","has","Tail"),
                                                    new Process("Cat","is a","Animal"),
                                                    new Process("Tiger","is a","Cat"),
                                                    new Process("Mouse","is a","Animal"),
                                                    new Process("Mouse","has","Tail"),
                                                    new Process("Mouse","has small","Size"),
                                                    new Process("Cat","hunt","Mouse", new Process("Cat","starving", null))
            };
            List<Process> pr2 = new List<Process> { new Process("Customer","has","Money"),
                                                    new Process("Customer","has","Basket"),
                                                    new Process("Basket","has","Product"),
                                                    new Process("Product","has","Code"),
                                                    new Process("Product", "has", "Price"),
                                                    new Process("BonusProduct","is a","Product"),
                                                    new Process("BonusProduct","has","Condition"),
                                                    new Process("CashDesk","has","Computer"),
                                                    new Process("CashDesk","start_session","Computer",new Process("Customer", "come", "CashDesk")),
                                                    new Process("Computer","show","Price", new Process("CashDesk", "read", "Code")),
                                                    new Process("Supermarket","has","Basket"),
                                                    new Process("Supermarket","has","Product"),
                                                    new Process("Supermarket","has","Computer"),
                                                    new Process("Customer","pay","CashDesk"),
                                                    new Process("Basket","add","BonusProduct",new Process("BonusProduct", "check", "Condition") )
            };

            //SNetwork net = new SNetwork(pr1);
            //KnowledgeBase.DataBase.CreateConnection("KnowledgeBase");
            //KnowledgeBase.DataBase.DataFromSemanticNetwork(net);
            //KnowledgeBase.DataBase.WriteToDataBase();
            //System.Diagnostics.Debug.WriteLine(net.GetDefinition("Animal"));
        }
    }
}