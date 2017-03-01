#! /usr/bin/perl

use strict;

my $in;
my $num_videos;
my $num_end_points;
my $num_requests;
my $num_caches;
my $max_cache_size;
my $ep;
my @line_parts;

my $filename = shift @ARGV;
my @lines;
my @videos;
my %endpoints = ();
my %requests = ();

open($in, $filename) or die("Can't open file $filename");

if( <$in> =~ /(\d+)\s(\d+)\s(\d+)\s(\d+)\s(\d+)/)
{
  $num_videos = $1;
  $num_end_points = $2;
  $num_requests = $3;
  $num_caches = $4;
  $max_cache_size = $5;
}

while(<$in>)
{
  chomp;
  push @lines, $_;
}

close($in);

@lines = reverse @lines;

@videos = split( / /, pop @lines);

for(my $x = 0; $x < $num_end_points; $x++)
{
  @line_parts = split(/ /, pop @lines);
  my $caches = $line_parts[1];

  $endpoints{$x} = {
    latency => $line_parts[0],
    num_caches => $caches,
    caches => {}
  };

  for(my $y = 0; $y < $caches; $y++)
  {
    @line_parts = split(/ /, pop @lines);

    $endpoints{$x}{caches}{$y} = {
      cache_nr => $line_parts[0],
      latency => $line_parts[1]
    };
  }
}

for(my $x = 0; $x < $num_requests; $x++)
{
  @line_parts = split(/ /, pop @lines);

  $requests{$x} = {
    num_requests => $line_parts[2],
    video => $line_parts[0],
    endpoint => $line_parts[1]
  };
}

for(my $x = 0; $x < $num_videos; $x++)
{
  print "insert into video(id, size) values($x, @videos[$x]);\n";
}

for(my $x = 0; $x < $num_end_points; $x++)
{
  print "insert into ep(id, latency, num_caches) values( $x, " . $endpoints{$x}{latency} . ", " . $endpoints{$x}{num_caches} . ");\n";

  for (my $y = 0; $y < $endpoints{$x}{num_caches}; $y++)
  {
    print "insert into cache(id, ep_id, cache_id, latency) values($y, $x, " . $endpoints{$x}{caches}{$y}{cache_nr} . ", " . $endpoints{$x}{caches}{$y}{latency} . ");\n";
  }
}

for (my $x = 0; $x < $num_requests; $x++)
{
  print "insert into requests(id, num_requests, video_id, ep_id) values( $x, ", $requests{$x}{num_requests} . ", " . $requests{$x}{video} . ", " . $requests{$x}{endpoint} . ");\n";
}

