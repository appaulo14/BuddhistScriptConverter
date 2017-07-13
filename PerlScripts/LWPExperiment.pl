#!/usr/bin/perl
use warnings;
use strict;
use utf8;
use Encode;
use LWP::UserAgent;
use Carp::Assert;
use HTML::TokeParser;
use List::MoreUtils qw(uniq);

#Set unicode output
binmode STDOUT, ":utf8";
#Create a user agent and attempt to get the content
my $ua = LWP::UserAgent->new();
assert(defined($ua));
$ua->proxy(['http','ftp'], 'http://pecsip:G0ATph33r@10.0.0.21:3128');
my $response = $ua->get('http://www.tipitaka.org/romn/cscd/abh01a.att0.xml');
#Proceed if successful
assert($response->is_success);
#print $response->decoded_content;  # or whatever
#Parse the retrieved html
my $content = $response->decoded_content();
assert(defined($content));
my $htmlParser = HTML::TokeParser->new(doc => \$content);
assert(defined($htmlParser));
#Parse out the links
my @romanWords;
while (my $token = $htmlParser->get_tag("p")) {
      #my $url = $token->[1]{href} || "-";
      my $text = $htmlParser->get_trimmed_text("/p");
      my @words = uniq(split(/ /, $text));
      $"="\n";
      print "@words\n";
}
  
