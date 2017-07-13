#!/usr/bin/perl
use warnings;
use strict;

use LWP::Simple;
use WWW::Robot;
use Test::Simple tests => 8;

#Create Robot
my $robot = new WWW::Robot('NAME'     =>  'PaliWordFetcher',
                           'VERSION'  =>  1.000,
                           'EMAIL'    =>  'fat.perl.hacker@gmail.com',
                           'VERBOSE'  =>  1);
#Set Proxy settings						   
$robot->proxy('http','http://pecsip:G0ATph33r@10.0.0.21:3128/');
$robot->proxy('ftp','ftp://pecsip:G0ATph33r@10.0.0.21:3128/');
#Get the agent being used by the Robot
my $agent = $robot->getAgent();
#Add the follow-url-test hook
my $follow_url_test_hook = sub {
    #Get paramters
    my ($robot, $hook, $url) = @_;
    #Filter out the obvious incorrects
    return 0 unless $url->scheme eq 'http';
    return 0 if $url =~ /\.(gif|jpg|png|xbm|au|wav|mpg)$/;
    #We're only interested in urls in a certain part of tipitaka.org
    return $url =~ m/\Awww[.]tipitaka[.]org/;
};
$robot->addHook('follow-url-test', $follow_url_test_hook);
#Add the invoke-on-contents
my $invoke_on_contents_hook = sub {
    my($robot, $hook, $url, $response, $structure) = @_;
    return unless $response->content_type eq 'text/xml';
};
$robot->addHook('invoke-on-contents',$invoke_on_contents_hook);

#Run tests
ok(defined(\&get), "LWP::Simple is defined");
ok(defined($robot) && ref $robot eq 'WWW::Robot', "Robot properly instantiated");
ok(defined($robot->getAttribute('NAME')),"Robot name accessible properly");
ok(defined($robot->getAttribute("VERSION")), "Robot version accessible properly");
ok(defined($robot->getAttribute("EMAIL")), "Robot email accessible properly");
ok($robot->getAttribute("VERBOSE") == 1, "Robot verbose mode set to true");
ok(defined($agent) && ref $agent eq 'LWP::RobotUA',"Agent retrieved sucessfully");
#$robot->run('http://www.tipitaka.org/romn/cscd');