using System;
using System.Text;
using Windows.Storage;

namespace PjacProto
{
    class AdaptiveCardTemplate
    {
        public static string ReadTextAsset(string fileName)
        {
            var assetUri = new Uri(string.Format("ms-appx:///Assets/{0}", fileName));
            StorageFile anjFile = StorageFile.GetFileFromApplicationUriAsync(assetUri).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            string result = FileIO.ReadTextAsync(anjFile).AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
            return result;
        }

        public const string TemplateBegin = @"
        {
            'type': 'AdaptiveCard',
            'body': [
                {
                    'type': 'ColumnSet',
                    'columns': [
                        {
                            'type': 'Column',
                            'items': [
                                {
                                    'type': 'TextBlock',
                                    'size': 'medium',
                                    'weight': 'bolder',
                                    'text': 'Recent Print Jobs'
                                }
                            ],
                            'width': 'stretch'
                        },
                        {
                            'type': 'Column',
                            'items': [
                                {
                                    'type': 'Image',
                                    'horizontalAlignment': 'right',
                                    'size': 'small',
                                    'url': 'http://png-4.findicons.com/files/icons/772/token_light/128/gear.png',
                                    'selectAction': {
                                        'type': 'Action.Submit',
                                        'data': 'Settings invoked'
                                    }
                                }
                            ]
                        }
                    ]
                },
        ";

        public const string TemplateEnd = @"
                ],
                '$schema': 'http://adaptivecards.io/schemas/adaptive-card.json',
                'version': '1.0'
            }
        ";

        public static string GetJobCard(string name, DateTime submitted, string status, string owner, int pages, DateTime completed)
        {
            string card = JobFormat;
            card = card.Replace("%%NAME%%", name);
            card = card.Replace("%%STATUS%%", status);
            card = card.Replace("%%OWNER%%", owner);
            card = card.Replace("%%PAGES%%", pages.ToString());
            // Submitted & completed are in this format: 2019-03-30T00:01:00Z
            card = card.Replace("%%DATE_SUBMITTED%%", submitted.ToString("s"));
            card = card.Replace("%%DATE_COMPLETED%%", completed.ToString("s"));
            return card;
        }

        const string JobFormat = @"
        {
            'type': 'Container',
            'items': [
                {
                    'type': 'ColumnSet',
                    'columns': [
                        {
                            'type': 'Column',
                            'items': [
                                {
                                    'type': 'Image',
                                    'url': 'http://icons.iconarchive.com/icons/dtafalonso/modern-xp/512/ModernXP-31-Filetype-Word-icon.png',
                                    'size': 'small'
                                }
                            ],
                            'width': 'auto'
                        },
                        {
                            'type': 'Column',
                            'items': [
                                {
                                    'type': 'TextBlock',
                                    'weight': 'bolder',
                                    'text': '%%NAME%%',
                                    'wrap': true
                                },
                                {
                                    'type': 'TextBlock',
                                    'spacing': 'None',
                                    'text': 'Submitted {{DATE(%%DATE_SUBMITTED%%Z,SHORT)}} at {{TIME(%%DATE_SUBMITTED%%Z)}} ',
                                    'isSubtle': true,
                                    'wrap': true
                                },
                                {
                                    'type': 'FactSet',
                                    'facts': [
                                        {
                                            'title': 'Status:',
                                            'value': '{%%STATUS%%}'
                                        },
                                        {
                                            'title': 'Owner:',
                                            'value': '{%%OWNER%%}'
                                        },
                                        {
                                            'title': 'Pages:',
                                            'value': '{%%PAGES%%}'
                                        },
                                        {
                                            'title': 'Completed:',
                                            'value': ' {{DATE(%%DATE_COMPLETED%%Z,SHORT)}} at {{TIME(%%DATE_COMPLETED%%Z)}} '
                                        }
                                    ]                
                                }
                            ],
                            'width': 'stretch'
                        }
                    ]
                }
            ]
        }
";



    }
}
