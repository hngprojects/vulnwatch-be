package com.vulnwatch.worker.scantests;

import com.vulnwatch.worker.scanners.dns.utility.DnsResolver;
import com.vulnwatch.worker.scanners.dns.models.ScanContext;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

import java.io.IOException;

import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

@SpringBootTest
public class DnsResolverTest {

    @Autowired
    DnsResolver dnsResolver;

    @Test
    public void shouldReturnFullScanContext() throws IOException {
        String domain = "nileuniversity.edu.ng";

        ScanContext scanContext = dnsResolver.resolveRecords(domain).join();

        assertNotNull(scanContext);
    }

}
