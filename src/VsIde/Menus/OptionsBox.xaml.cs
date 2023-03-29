//namespace LspAntlr
//{
//    using Microsoft.VisualStudio.ComponentModelHost;
//    using Microsoft.VisualStudio.Shell;
//    using Microsoft.VisualStudio.Text.Classification;
//    using Options;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Windows;
//    using System.Windows.Controls;
//    using System.Windows.Media;

//    public partial class OptionsBox : Window
//    {
//        public OptionsBox()
//        {
//            InitializeComponent();
//            visible_server_window.IsChecked = Option.GetBoolean("VisibleServerWindow");
//            restricted_directory.IsChecked = Option.GetBoolean("RestrictedDirectory");
//            generate_visitor_listener.IsChecked = Option.GetBoolean("GenerateVisitorListener");
//            corpus_location.Text = Option.GetString("CorpusLocation");
//            incremental_reformat.IsChecked = Option.GetBoolean("IncrementalReformat");
//            override_antlr.IsChecked = Option.GetBoolean("OverrideAntlrPluggins");
//            opt_in_reporting.IsChecked = Option.GetBoolean("OptInLogging");
//            enable_completion.IsChecked = Option.GetBoolean("EnableCompletion");
//            semantic_highlighting.IsChecked = Option.GetBoolean("SemanticHighlighting");

//            // Get list of all equivalence classes from VS.
//            IComponentModel componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
//            var service = componentModel.GetService<Microsoft.VisualStudio.Text.Classification.IClassificationFormatMapService>();
//            var classificationFormatMap = service.GetClassificationFormatMap(category: "text");
//            var list_of_formats = classificationFormatMap.CurrentPriorityOrder;
//            var list_of_names = new List<string>();
//            foreach (var p in list_of_formats)
//            {
//                if (p == null) continue;
//                var name = p.Classification;
//                if (name.Contains("Antlr")) continue;
//                list_of_names.Add(name);
//            }
//            list_of_names.Sort();

//            {
//                var text = Option.GetString("AntlrNonterminalDef");
//                nonterminal_def_color.SelectedValue = text;
//                foreach (var n in list_of_names) nonterminal_def_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d1.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrNonterminalRef");
//                nonterminal_ref_color.SelectedValue = text;
//                foreach (var n in list_of_names) nonterminal_ref_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d2.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrTerminalDef");
//                terminal_def_color.SelectedValue = text;
//                foreach (var n in list_of_names) terminal_def_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d3.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrTerminalRef");
//                terminal_ref_color.SelectedValue = text;
//                foreach (var n in list_of_names) terminal_ref_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d4.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrComment");
//                comment_color.SelectedValue = text;
//                foreach (var n in list_of_names) comment_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d5.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrKeyword");
//                keyword_color.SelectedValue = text;
//                foreach (var n in list_of_names) keyword_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d6.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrLiteral");
//                literal_color.SelectedValue = text;
//                foreach (var n in list_of_names) literal_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d7.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrModeDef");
//                mode_def_color.SelectedValue = text;
//                foreach (var n in list_of_names) mode_def_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d8.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrModeRef");
//                mode_ref_color.SelectedValue = text;
//                foreach (var n in list_of_names) mode_ref_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d9.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrChannelDef");
//                mode_ref_color.SelectedValue = text;
//                foreach (var n in list_of_names) channel_def_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d10.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrChannelRef");
//                channel_ref_color.SelectedValue = text;
//                foreach (var n in list_of_names) channel_ref_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d11.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrPunctuation");
//                punctuation_color.SelectedValue = text;
//                foreach (var n in list_of_names) punctuation_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d12.Fill = brush;
//            }
//            {
//                var text = Option.GetString("AntlrOperator");
//                operator_color.SelectedValue = text;
//                foreach (var n in list_of_names) operator_color.Items.Add(n);
//                var color = GetColor(text);
//                var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//                var brush = new SolidColorBrush();
//                brush.Color = media_color;
//                d13.Fill = brush;
//            }
//        }

//        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
//        {
//            OptionsBox inputDialog = this;
//            Option.SetBoolean("VisibleServerWindow", inputDialog.visible_server_window.IsChecked ?? false);
//            Option.SetBoolean("RestrictedDirectory", inputDialog.restricted_directory.IsChecked ?? false);
//            Option.SetBoolean("GenerateVisitorListener", inputDialog.generate_visitor_listener.IsChecked ?? false);
//            Option.SetString("CorpusLocation", inputDialog.corpus_location.Text);
//            Option.SetBoolean("IncrementalReformat", inputDialog.incremental_reformat.IsChecked ?? false);
//            Option.SetBoolean("OverrideAntlrPluggins", inputDialog.override_antlr.IsChecked ?? false);
//            Option.SetBoolean("OptInLogging", inputDialog.opt_in_reporting.IsChecked ?? false);
//            Option.SetBoolean("EnableCompletion", inputDialog.enable_completion.IsChecked ?? false);
//            Option.SetBoolean("SemanticHighlighting", inputDialog.semantic_highlighting.IsChecked ?? false);

//            Option.SetString("AntlrNonterminalDef", inputDialog.nonterminal_def_color.Text);
//            Option.SetString("AntlrNonterminalRef", inputDialog.nonterminal_ref_color.Text);
//            Option.SetString("AntlrTerminalDef", inputDialog.terminal_def_color.Text);
//            Option.SetString("AntlrTerminalRef", inputDialog.terminal_ref_color.Text);
//            Option.SetString("AntlrComment", inputDialog.comment_color.Text);
//            Option.SetString("AntlrKeyword", inputDialog.keyword_color.Text);
//            Option.SetString("AntlrLiteral", inputDialog.literal_color.Text);
//            Option.SetString("AntlrTerminalRef", inputDialog.terminal_ref_color.Text);
//            Option.SetString("AntlrModeDef", inputDialog.mode_def_color.Text);
//            Option.SetString("AntlrModeRef", inputDialog.mode_ref_color.Text);
//            Option.SetString("AntlrChannelDef", inputDialog.channel_def_color.Text);
//            Option.SetString("AntlrChannelRef", inputDialog.channel_ref_color.Text);
//            Option.SetString("AntlrPunctuation", inputDialog.punctuation_color.Text);
//            Option.SetString("AntlrOperator", inputDialog.operator_color.Text);

//            Close();
//        }

//        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
//        {
//            Close();
//        }

//        private System.Windows.Media.Color GetColor(string text)
//        {
//            IComponentModel componentModel = Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
//            var service = componentModel.GetService<Microsoft.VisualStudio.Text.Classification.IClassificationFormatMapService>();
//            var classificationFormatMap = service.GetClassificationFormatMap(category: "text");
//            var list_of_formats = classificationFormatMap.CurrentPriorityOrder;
//            IClassificationType selected = list_of_formats.Where(t => { if (t != null) return t.Classification == text; return false; }).FirstOrDefault();
//            if (selected == null) return System.Windows.Media.Color.FromArgb(0,0,0,0);
//            Microsoft.VisualStudio.Text.Formatting.TextFormattingRunProperties identifierProperties = classificationFormatMap.GetExplicitTextProperties(selected);
//            var brush = identifierProperties.ForegroundBrush;
//            var tttt = brush.GetType();
//            if (brush is SolidColorBrush)
//            {
//                var b = brush as SolidColorBrush;
//                System.Windows.Media.Color color = b.Color;
//                return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            }
//            return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
//        }

//        private void nonterminal_def_color_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d1.Fill = brush;
//     }

//        private void nonterminal_ref_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d2.Fill = brush;
//        }

//        private void terminal_def_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d3.Fill = brush;
//        }

//        private void terminal_ref_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d4.Fill = brush;
//        }

//        private void comment_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d5.Fill = brush;
//        }

//        private void keyword_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d6.Fill = brush;
//        }

//        private void literal_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d7.Fill = brush;
//        }

//        private void mode_def_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d8.Fill = brush;
//        }

//        private void mode_ref_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d9.Fill = brush;
//        }

//        private void channel_def_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d10.Fill = brush;
//        }

//        private void channel_ref_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d11.Fill = brush;
//        }

//        private void punctuation_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d12.Fill = brush;
//        }

//        private void operator_color_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            string text = (sender as ComboBox).SelectedItem as string;
//            var color = GetColor(text);
//            var media_color = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
//            var brush = new SolidColorBrush();
//            brush.Color = media_color;
//            d13.Fill = brush;
//        }
//    }
//}
