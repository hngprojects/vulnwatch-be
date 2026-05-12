package com.vulnwatch.worker.scantests;

import static org.junit.jupiter.api.Assertions.assertNotNull;

import com.vulnwatch.worker.scanners.dns.models.ScanContext;
import com.vulnwatch.worker.scanners.dns.utility.DnsResolver;
import com.vulnwatch.worker.scanners.dns.utility.RuleEngine;
import java.io.IOException;
import java.util.Map;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;

@SpringBootTest
public class RuleEngineTest {

  @Autowired private DnsResolver dnsResolver;

  @Autowired private RuleEngine ruleEngine;

  @Test
  public void shouldEngineSuccessfully() throws IOException {
    String domain = "amazon.com";

    ScanContext scanContext = dnsResolver.resolveRecords(domain).join();

    Map<String, Object> scanResult = ruleEngine.scanJob(scanContext);

    assertNotNull(scanResult);
  }
}
