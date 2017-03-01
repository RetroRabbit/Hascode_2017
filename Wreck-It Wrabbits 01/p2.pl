#! /usr/bin/perl
use strict;

my $in;
my $max_cache_size;
my $filename = shift @ARGV;
my %caches;
my %eps;
my $cache_id;
my $vid_id;
my $size;
my $ep_id;
my $num_caches;

# get the max cache size
open($in, $filename) or die("Can't open file $filename. Reason: $!");

if( <$in> =~ /\d+\s\d+\s\d+\s\d+\s(\d+)/)
{
  $max_cache_size = $1;
}

close($in);

# parse the sql stuff
# ep_id\tcache_id\tvid_id\tscore\tsize
open($in, "/tmp/res") or die("can't open file res. Reason: $!");

while(<$in>)
{
  chomp;

  if ($_ =~ /(\d+)\t(\d+)\t(\d+)\t\d+\t(\d+)/)
  {
    $ep_id = $1;
    $cache_id = $2;
    $vid_id = $3;
    $size = $4;

    if ( !exists $caches{$cache_id} )
    {
      $caches{$cache_id} = {
        videos => {},
        total => 0
      }
    }

    if ( !exists $eps{$ep_id})
    {
      $eps{$ep_id} = {};
    }

    # respect the max cache size
    if ($caches{$cache_id}{total} + $size <= $max_cache_size)
    {
      # check for dups on the ep level and cache lvl
      if (! exists $eps{$ep_id}{$vid_id} && ! exists $caches{$cache_id}{videos}{$vid_id})
      {
        $caches{$cache_id}{videos}{$vid_id} = 1;
        $caches{$cache_id}{total} += $size;
        $eps{$ep_id}{$vid_id} = 1;
      }
    }
  }
}

close($in);

$num_caches = keys %caches;
print "$num_caches\n";

for my $key (sort keys %caches)
{
  print $key;

  for my $v ( keys %{$caches{$key}{videos}})
  {
    print " $v";
  }

  print "\n"
}
