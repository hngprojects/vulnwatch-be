package com.vulnwatch.worker.scantests.rules;

import com.vulnwatch.worker.scanners.dns.models.Finding;
import com.vulnwatch.worker.scanners.dns.models.ScanContext;
import com.vulnwatch.worker.scanners.dns.rules.IpRules;
import com.vulnwatch.worker.scanners.dns.utility.DnsResolver;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import java.io.IOException;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertNotNull;

@SpringBootTest
public class IpRulesTest {

    @Autowired
    private IpRules ipRules;

    @Autowired
    private DnsResolver dnsResolver;

    @Test
    public void testIpRules() throws IOException {
        String domain = "google.com";

        ScanContext scanContext = dnsResolver.resolveRecords(domain).join();

        List<Finding> findings = ipRules.evaluate(scanContext);

        assertNotNull(findings);
    }
}
