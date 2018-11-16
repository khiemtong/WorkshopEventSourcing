﻿using System;
using System.Threading.Tasks;
using Marketplace.Framework;
using Marketplace.Infrastructure.RavenDB;
using Raven.Client.Documents.Session;
using static Marketplace.Domain.ClassifiedAds.Events;

namespace Marketplace.Modules.ClassifiedAds.Projections
{
    public class AvailableClassifiedAdsProjection : Projection
    {
        Func<IAsyncDocumentSession> GetSession { get; }

        public AvailableClassifiedAdsProjection(Func<IAsyncDocumentSession> getSession) => GetSession = getSession;

        public override Task Handle(object e)
        {
            switch (e)
            {
                case V1.ClassifiedAdRegistered x:
                    return GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc =>
                        {
                            doc.Owner = x.Owner;
                            doc.ClassifiedAdId = x.ClassifiedAdId;
                        });

                case V1.ClassifiedAdTitleChanged x:
                    return GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.Title = x.Title);

                case V1.ClassifiedAdTextChanged x:
                    return GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.Text = x.Text);

                case V1.ClassifiedAdPriceChanged x:
                    return GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.Price = x.Price);

                case V1.ClassifiedAdPublished x:
                    return GetSession.ThenSave<AvailableClassifiedAd>(
                        AvailableClassifiedAd.Id(x.ClassifiedAdId), doc => doc.PublishedAt = x.PublishedAt);

                case V1.ClassifiedAdSold x:
                    return GetSession.ThenDelete(AvailableClassifiedAd.Id(x.ClassifiedAdId));

                case V1.ClassifiedAdRemoved x:
                    return GetSession.ThenDelete(AvailableClassifiedAd.Id(x.ClassifiedAdId));

                default:
                    return Task.CompletedTask;
            }
        }

        public override bool CanHandle(object e)
            => e is V1.ClassifiedAdRegistered
            || e is V1.ClassifiedAdPublished
            || e is V1.ClassifiedAdTitleChanged
            || e is V1.ClassifiedAdTextChanged
            || e is V1.ClassifiedAdPriceChanged
            || e is V1.ClassifiedAdRemoved
            || e is V1.ClassifiedAdSold;

        public class AvailableClassifiedAd
        {
            public static string Id(Guid id) => $"AvailableClassifiedAds/{id}";

            public Guid ClassifiedAdId { get; set; }
            public string Title { get; set; }
            public double Price { get; set; }
            public DateTimeOffset? PublishedAt { get; set; }
            public string Text { get; set; }
            public Guid Owner { get; set; }
        }
    }
}
