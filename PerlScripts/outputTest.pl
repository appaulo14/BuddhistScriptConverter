#!/usr/bin/perl
use warnings;
use strict;
use LWP::UserAgent;
use Carp::Assert;
use HTML::TokeParser;

my $ua=LWP::UserAgent->new();
$ua->proxy(['http','ftp'], 'http://pecsip:G0ATph33r@10.0.0.21:3128');
my $response = $ua->get('http://www.tipitaka.org/deva/cscd');
#Proceed if successful
assert($response->is_success);
#print $response->decoded_content;  # or whatever
#Parse the retrieved html
my $content = $response->decoded_content();
assert(defined($content));
my $htmlParser = HTML::TokeParser->new(doc => \$content);
assert(defined($htmlParser));
#Parse out the links
open my $fhRomanLinks, ">", "devaLinks.txt" or die "$!";
while (my $token = $htmlParser->get_tag("a")) {
      #my $url = $token->[1]{href} || "-";
      my $link = $htmlParser->get_trimmed_text("/a");
      #Write to our list of links if it is an xml documet
      if ($link =~/[.]xml\z/ && $link !~ /[.]toc[.]xml\z/){
        print $fhRomanLinks "http://www.tipitaka.org/deva/cscd/$link\n";
      }
}