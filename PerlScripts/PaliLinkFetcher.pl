#!/usr/bin/perl
use strict;
use warnings;
use Moose::Autobox;
use HTML::TokeParser;
use Carp;
use Carp::Assert;
use utf8;
use English qw( -no_match_vars ) ;  # Avoids regex performance penalty
require "Proxied_User_Agent.pm";

#my $url = 'C:/Users/PaulCain/Downloads/khmr_cscd.htm';
my $url = "http://www.tipitaka.org/khmr/cscd";
my $ua = LWP::ProxiedUserAgent->new();
open my $fhLinks, ">:encoding(UTF-8)", "khmerLinks.txt" or confess "$OS_ERROR";
#Attempt to get the content from $url
 my $response = $ua->get($url);
 $response->is_success or confess "Response failed for $url";
 #If sucessful parse the content
 my $content = $response->decoded_content();
 my $htmlParser = HTML::TokeParser->new(doc => \$content);
 #Parse the words out of the content
 my $count = 0;
 while (my $token = $htmlParser->get_tag("a")) {
    my $text = $htmlParser->get_trimmed_text("/a");
    print $fhLinks "$url/$text\n" if ($text=~m/[.]xml\z/ and $text !~ m/[.]toc[.]xml\z/);
 }
