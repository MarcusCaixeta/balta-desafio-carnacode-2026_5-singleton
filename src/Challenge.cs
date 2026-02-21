using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatternChallenge
{
    public interface IPrototype<T>
    {
        T Clone();
    }

    public class Margins : IPrototype<Margins>
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Margins Clone()
        {
            return new Margins
            {
                Top = Top,
                Bottom = Bottom,
                Left = Left,
                Right = Right
            };
        }
    }

    public class DocumentStyle : IPrototype<DocumentStyle>
    {
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string HeaderColor { get; set; }
        public string LogoUrl { get; set; }
        public Margins PageMargins { get; set; }

        public DocumentStyle Clone()
        {
            return new DocumentStyle
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                HeaderColor = HeaderColor,
                LogoUrl = LogoUrl,
                PageMargins = PageMargins?.Clone()
            };
        }
    }

    public class Section : IPrototype<Section>
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsEditable { get; set; }
        public List<string> Placeholders { get; set; } = new();

        public Section Clone()
        {
            return new Section
            {
                Name = Name,
                Content = Content,
                IsEditable = IsEditable,
                Placeholders = new List<string>(Placeholders)
            };
        }
    }

    public class ApprovalWorkflow : IPrototype<ApprovalWorkflow>
    {
        public List<string> Approvers { get; set; } = new();
        public int RequiredApprovals { get; set; }
        public int TimeoutDays { get; set; }

        public ApprovalWorkflow Clone()
        {
            return new ApprovalWorkflow
            {
                RequiredApprovals = RequiredApprovals,
                TimeoutDays = TimeoutDays,
                Approvers = new List<string>(Approvers)
            };
        }
    }

    public class DocumentTemplate : IPrototype<DocumentTemplate>
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public List<Section> Sections { get; set; } = new();
        public DocumentStyle Style { get; set; }
        public List<string> RequiredFields { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
        public ApprovalWorkflow Workflow { get; set; }
        public List<string> Tags { get; set; } = new();

        public DocumentTemplate Clone()
        {
            return new DocumentTemplate
            {
                Title = Title,
                Category = Category,
                Style = Style?.Clone(),
                Workflow = Workflow?.Clone(),
                Sections = Sections.Select(s => s.Clone()).ToList(),
                RequiredFields = new List<string>(RequiredFields),
                Metadata = new Dictionary<string, string>(Metadata),
                Tags = new List<string>(Tags)
            };
        }
    }

    public class TemplateRegistry
    {
        private Dictionary<string, DocumentTemplate> templates = new();

        public void Register(string name, DocumentTemplate template)
        {
            templates[name] = template;
        }

        public DocumentTemplate Create(string name)
        {
            if (!templates.ContainsKey(name))
                throw new Exception($"Template {name} não encontrado");

            return templates[name].Clone();
        }
    }

    public class DocumentService
    {
        private TemplateRegistry registry = new();

        public DocumentService()
        {
            registry.Register("service_contract", BuildServiceContractTemplate());
            registry.Register("consulting_contract", BuildConsultingContractTemplate());
        }

        private DocumentTemplate BuildServiceContractTemplate()
        {
            Console.WriteLine("Inicializando template base de contrato de serviço...");

            return new DocumentTemplate
            {
                Title = "Contrato de Prestação de Serviços",
                Category = "Contratos",

                Style = new DocumentStyle
                {
                    FontFamily = "Arial",
                    FontSize = 12,
                    HeaderColor = "#003366",
                    LogoUrl = "https://company.com/logo.png",
                    PageMargins = new Margins
                    {
                        Top = 2,
                        Bottom = 2,
                        Left = 3,
                        Right = 3
                    }
                },

                Workflow = new ApprovalWorkflow
                {
                    RequiredApprovals = 2,
                    TimeoutDays = 5,
                    Approvers = new List<string>
                    {
                        "gerente@empresa.com",
                        "juridico@empresa.com"
                    }
                },

                Sections = new List<Section>
                {
                    new Section
                    {
                        Name="Cláusula 1 - Objeto",
                        Content="O presente contrato tem por objeto...",
                        IsEditable=true
                    },
                    new Section
                    {
                        Name="Cláusula 2 - Prazo",
                        Content="O prazo de vigência será de...",
                        IsEditable=true
                    },
                    new Section
                    {
                        Name="Cláusula 3 - Valor",
                        Content="O valor total do contrato é de...",
                        IsEditable=true
                    }
                },

                RequiredFields = new List<string>
                {
                    "NomeCliente",
                    "CPF",
                    "Endereco"
                },

                Tags = new List<string>
                {
                    "contrato",
                    "servicos"
                },

                Metadata = new Dictionary<string, string>
                {
                    {"Versao","1.0"},
                    {"Departamento","Comercial"},
                    {"UltimaRevisao", DateTime.Now.ToString()}
                }
            };
        }

        private DocumentTemplate BuildConsultingContractTemplate()
        {
            var template = BuildServiceContractTemplate().Clone();

            template.Title = "Contrato de Consultoria";
            template.Tags.Add("consultoria");

            template.Sections[0].Content =
                "O presente contrato de consultoria tem por objeto...";

            return template;
        }

        public DocumentTemplate Create(string templateName)
        {
            return registry.Create(templateName);
        }

        public void DisplayTemplate(DocumentTemplate template)
        {
            Console.WriteLine($"\n=== {template.Title} ===");
            Console.WriteLine($"Categoria: {template.Category}");
            Console.WriteLine($"Seções: {template.Sections.Count}");
            Console.WriteLine($"Campos obrigatórios: {string.Join(", ", template.RequiredFields)}");
            Console.WriteLine($"Aprovadores: {string.Join(", ", template.Workflow.Approvers)}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Templates de Documentos (Prototype Pattern) ===\n");

            var service = new DocumentService();

            Console.WriteLine("Criando 5 contratos de serviço via clone...\n");

            var start = DateTime.Now;

            for (int i = 1; i <= 5; i++)
            {
                var contract = service.Create("service_contract");

                // Personalização após clonagem
                contract.Title = $"Contrato #{i} - Cliente {i}";
                contract.Metadata["Cliente"] = $"Cliente {i}";
            }

            var elapsed = (DateTime.Now - start).TotalMilliseconds;

            Console.WriteLine($"Tempo total: {elapsed}ms");

            var consultingContract = service.Create("consulting_contract");

            service.DisplayTemplate(consultingContract);
        }
    }
}