using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace VideoClassification
{
    public class VideoDocModel
    {
        [JsonProperty("partition")]
        public string Partition { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("privacyMode")]
        public string PrivacyMode { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("accountId")]
        public Guid AccountId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("isOwned")]
        public bool IsOwned { get; set; }

        [JsonProperty("isEditable")]
        public bool IsEditable { get; set; }

        [JsonProperty("isBase")]
        public bool IsBase { get; set; }

        [JsonProperty("durationInSeconds")]
        public long DurationInSeconds { get; set; }

        [JsonProperty("summarizedInsights")]
        public SummarizedInsights SummarizedInsights { get; set; }

        [JsonProperty("videos")]
        public Video[] Videos { get; set; }

        [JsonProperty("videosRanges")]
        public VideosRange[] VideosRanges { get; set; }
    }

    public partial class SummarizedInsights
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("privacyMode")]
        public string PrivacyMode { get; set; }

        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("thumbnailVideoId")]
        public string ThumbnailVideoId { get; set; }

        [JsonProperty("thumbnailId")]
        public Guid ThumbnailId { get; set; }

        [JsonProperty("faces")]
        public object[] Faces { get; set; }

        [JsonProperty("keywords")]
        public LabelElement[] Keywords { get; set; }

        [JsonProperty("sentiments")]
        public object[] Sentiments { get; set; }

        [JsonProperty("emotions")]
        public object[] Emotions { get; set; }

        [JsonProperty("audioEffects")]
        public object[] AudioEffects { get; set; }

        [JsonProperty("labels")]
        public LabelElement[] Labels { get; set; }

        [JsonProperty("brands")]
        public SummarizedInsightsBrand[] Brands { get; set; }

        [JsonProperty("topics")]
        public object[] Topics { get; set; }
    }

    public partial class SummarizedInsightsBrand
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("referenceUrl")]
        public Uri ReferenceUrl { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("seenDuration")]
        public double SeenDuration { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("appearances")]
        public Appearance[] Appearances { get; set; }
    }

    public partial class Appearance
    {
        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("endTime")]
        public string EndTime { get; set; }

        [JsonProperty("startSeconds")]
        public double StartSeconds { get; set; }

        [JsonProperty("endSeconds")]
        public double EndSeconds { get; set; }
    }

    public partial class Duration
    {
        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("seconds")]
        public double Seconds { get; set; }
    }

    public partial class LabelElement
    {
        [JsonProperty("isTranscript", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTranscript { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("appearances")]
        public Appearance[] Appearances { get; set; }
    }

    public partial class Speaker
    {
        [JsonProperty("1")]
        public long The1 { get; set; }
    }

    public partial class Video
    {
        [JsonProperty("accountId")]
        public Guid AccountId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("moderationState")]
        public string ModerationState { get; set; }

        [JsonProperty("reviewState")]
        public string ReviewState { get; set; }

        [JsonProperty("privacyMode")]
        public string PrivacyMode { get; set; }

        [JsonProperty("processingProgress")]
        public string ProcessingProgress { get; set; }

        [JsonProperty("failureCode")]
        public string FailureCode { get; set; }

        [JsonProperty("failureMessage")]
        public string FailureMessage { get; set; }

        [JsonProperty("externalId")]
        public object ExternalId { get; set; }

        [JsonProperty("externalUrl")]
        public object ExternalUrl { get; set; }

        [JsonProperty("metadata")]
        public object Metadata { get; set; }

        [JsonProperty("insights")]
        public Insights Insights { get; set; }

        [JsonProperty("thumbnailId")]
        public Guid ThumbnailId { get; set; }

        [JsonProperty("publishedUrl")]
        public Uri PublishedUrl { get; set; }

        [JsonProperty("publishedProxyUrl")]
        public object PublishedProxyUrl { get; set; }

        [JsonProperty("viewToken")]
        public string ViewToken { get; set; }

        [JsonProperty("detectSourceLanguage")]
        public bool DetectSourceLanguage { get; set; }

        [JsonProperty("sourceLanguage")]
        public string SourceLanguage { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("indexingPreset")]
        public string IndexingPreset { get; set; }

        [JsonProperty("linguisticModelId")]
        public Guid LinguisticModelId { get; set; }

        [JsonProperty("personModelId")]
        public Guid PersonModelId { get; set; }

        [JsonProperty("isAdult")]
        public bool IsAdult { get; set; }
    }

    public partial class Insights
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("sourceLanguage")]
        public string SourceLanguage { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("transcript")]
        public TranscriptElement[] Transcript { get; set; }

        [JsonProperty("ocr")]
        public Ocr[] Ocr { get; set; }

        [JsonProperty("keywords")]
        public TranscriptElement[] Keywords { get; set; }

        [JsonProperty("labels")]
        public Label[] Labels { get; set; }
       
    }

    public partial class Block
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    //public partial class Instance
    //{
    //    [JsonProperty("adjustedStart")]
    //    public string AdjustedStart { get; set; }

    //    [JsonProperty("adjustedEnd")]
    //    public string AdjustedEnd { get; set; }

    //    [JsonProperty("start")]
    //    public string Start { get; set; }

    //    [JsonProperty("end")]
    //    public string End { get; set; }

    //    [JsonProperty("brandType", NullValueHandling = NullValueHandling.Ignore)]
    //    public string BrandType { get; set; }

    //    [JsonProperty("confidence", NullValueHandling = NullValueHandling.Ignore)]
    //    public double? Confidence { get; set; }

    //    [JsonProperty("thumbnailId", NullValueHandling = NullValueHandling.Ignore)]
    //    public Guid? ThumbnailId { get; set; }
    //}

    public partial class InsightsBrand
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("referenceUrl")]
        public Uri ReferenceUrl { get; set; }

        [JsonProperty("referenceType")]
        public string ReferenceType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tags")]
        public object[] Tags { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("isCustom")]
        public bool IsCustom { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    public partial class FramePattern
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("patternType")]
        public string PatternType { get; set; }

        [JsonProperty("confidence")]
        public long Confidence { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    public partial class TranscriptElement
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }

        [JsonProperty("speakerId", NullValueHandling = NullValueHandling.Ignore)]
        public long? SpeakerId { get; set; }
    }

    public partial class Label
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }

        [JsonProperty("referenceId", NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceId { get; set; }
    }

    public partial class Ocr
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("left")]
        public long Left { get; set; }

        [JsonProperty("top")]
        public long Top { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    public partial class Shot
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("tags")]
        public object[] Tags { get; set; }

        [JsonProperty("keyFrames")]
        public Block[] KeyFrames { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    public partial class SpeakerElement
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    public partial class TextualContentModeration
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("bannedWordsCount")]
        public long BannedWordsCount { get; set; }

        [JsonProperty("bannedWordsRatio")]
        public long BannedWordsRatio { get; set; }

        [JsonProperty("instances")]
        public object[] Instances { get; set; }
    }

    public partial class VisualContentModeration
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("adultScore")]
        public double AdultScore { get; set; }

        [JsonProperty("racyScore")]
        public double RacyScore { get; set; }

        //[JsonProperty("instances")]
        //public Instance[] Instances { get; set; }
    }

    public partial class VideosRange
    {
        [JsonProperty("videoId")]
        public string VideoId { get; set; }

        [JsonProperty("range")]
        public Range Range { get; set; }
    }

    public partial class Range
    {
        [JsonProperty("start")]
        public string Start { get; set; }

        [JsonProperty("end")]
        public string End { get; set; }
    }



}
