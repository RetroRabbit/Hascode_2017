drop table if exists requests;
create table requests(id bigint, num_requests bigint, video_id bigint, ep_id bigint);
drop table if exists cache;
create table cache(id bigint, ep_id bigint, cache_id bigint, latency bigint);
drop table if exists ep;
create table ep(id bigint, latency bigint, num_caches bigint);
drop table if exists video;
create table video(id bigint, size bigint);
